using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Logging;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
    public class CruiseServer : ICruiseServer
    {
        #region Fields

        private readonly IConfigurationService configurationService;

        private bool disposed;

        private readonly ICruiseManager manager;

        private readonly ManualResetEvent monitor = new ManualResetEvent(true);

        private readonly IProjectIntegratorListFactory projectIntegratorListFactory;

        private IProjectIntegratorList projectIntegrators;

        private readonly IProjectSerializer projectSerializer;

        #endregion

        #region Constructors

        public CruiseServer(IConfigurationService configurationService, IProjectIntegratorListFactory projectIntegratorListFactory, IProjectSerializer projectSerializer)
        {
            this.configurationService = configurationService;
            this.configurationService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(Restart));
            this.projectIntegratorListFactory = projectIntegratorListFactory;

            // ToDo - get rid of manager, maybe
            manager = new CruiseManager(this);

            // By default, no integrators are running
            this.projectSerializer = projectSerializer;

            CreateIntegrators();
        }

        #endregion

        #region Properties

        public ICruiseManager CruiseManager
        {
            get { return manager; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Abort all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
        /// </summary>
        public void Abort()
        {
            Log.Info("Aborting CruiseControl.NET Server");
            AbortIntegrators();
            monitor.Set();
        }

        // ToDo - test
        public void AddProject(string serializedProject)
        {
            Log.Info("Adding project - " + serializedProject);
            try
            {
                IConfiguration configuration = configurationService.Load();
                Project project = projectSerializer.Deserialize(serializedProject);
                configuration.AddProject(project);
                project.Initialize();
                configurationService.Save(configuration);
            }
            catch (ApplicationException e)
            {
                Log.Warning(e);
                throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
            }
        }

        // ToDo - test
        // ToDo - when we decide how to handle configuration changes, do more here (like stopping/waiting for project, returning asynchronously, etc.)
        public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
            Log.Info("Deleting project - " + projectName);
            try
            {
                IConfiguration configuration = configurationService.Load();
                configuration.Projects[projectName].Purge(purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
                configuration.DeleteProject(projectName);
                configurationService.Save(configuration);
            }
            catch (Exception e)
            {
                Log.Warning(e);
                throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
            }
        }

        public bool ForceBuild(string projectName, ForceFilterClientInfo[] clientInfo)
        {
            return GetIntegrator(projectName).ForceBuild(clientInfo);
        }

        

        public string[] GetBuildNames(string projectName)
        {
			string logDirectory = this.GetBuildLogDirectory(projectName);
            if (!Directory.Exists(logDirectory))
            {
                Log.Warning("Log Directory [ " + logDirectory + " ] does not exist. Are you sure any builds have completed?");
                return new string[0];
            }
            string[] logFileNames = LogFileUtil.GetLogFileNames(logDirectory);
            Array.Reverse(logFileNames);
            return logFileNames;
        }

        public ExternalLink[] GetExternalLinks(string projectName)
        {
            return GetIntegrator(projectName).Project.ExternalLinks;
        }

        public string GetLatestBuildName(string projectName)
        {
            string[] buildNames = GetBuildNames(projectName);
            if (buildNames.Length > 0)
            {
                return buildNames[0];
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetLog(string projectName, string buildName)
        {
			string logDirectory = this.GetBuildLogDirectory(projectName);
            if (!Directory.Exists(logDirectory))
            {
                Log.Warning("Log Directory [ " + logDirectory + " ] does not exist. Are you sure any builds have completed?");
                return "";
            }
            using (StreamReader sr = new StreamReader(Path.Combine(logDirectory, buildName)))
            {
                return sr.ReadToEnd();
            }
        }

        public string[] GetMostRecentBuildNames(string projectName, int buildCount)
        {
            // TODO - this is a hack - I'll tidy it up later - promise! :) MR
            string[] buildNames = GetBuildNames(projectName);
            ArrayList buildNamesToReturn = new ArrayList();
            for (int i = 0; i < ((buildCount < buildNames.Length) ? buildCount : buildNames.Length); i++)
            {
                buildNamesToReturn.Add(buildNames[i]);
            }
            return (string[])buildNamesToReturn.ToArray(typeof(string));
        }

        // ToDo - this done TDD
        public string GetProject(string name)
        {
            Log.Info("Getting project - " + name);
            return new NetReflectorProjectSerializer().Serialize((Project)configurationService.Load().Projects[name]);
        }

        public ProjectStatus[] GetProjectStatus()
        {
            ArrayList projectStatusList = new ArrayList();
            foreach (IProjectIntegrator integrator in projectIntegrators)
            {
                Project project = (Project)integrator.Project;
                projectStatusList.Add(new ProjectStatus(integrator.State,
                                                        project.LatestBuildStatus,
                                                        project.CurrentActivity,
                                                        project.Name,
                                                        project.WebURL,
                                                        project.LastIntegrationResult.StartTime,
                                                        project.LastIntegrationResult.Label,
                                                        project.LastIntegrationResult.LastSuccessfulIntegrationLabel,
                                                        integrator.Trigger.NextBuild));
            }

            return (ProjectStatus[])projectStatusList.ToArray(typeof(ProjectStatus));
        }

        public ProjectStatus GetProjectStatus(string projectName)
        {
            IProjectIntegrator integrator = projectIntegrators[projectName];
            Project project = (Project)integrator.Project;
            return new ProjectStatus(integrator.State,
                project.LatestBuildStatus,
                project.CurrentActivity,
                project.Name,
                project.WebURL,
                project.LastIntegrationResult.StartTime,
                project.LastIntegrationResult.Label,
                project.LastIntegrationResult.LastSuccessfulIntegrationLabel,
                integrator.Trigger.NextBuild);
        }

        // ToDo - test
        public string GetServerLog()
        {
            return new ServerLogFileReader().Read();
        }

        public string GetVersion()
        {
            Log.Info("Returning version number");
            try
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (ApplicationException e)
            {
                Log.Warning(e);
                throw new CruiseControlException("Failed to get project version . Exception was - " + e.Message);
            }
        }

        /// <summary>
        /// Restart server by stopping all integrators, creating a new set of integrators from Configuration and then starting them.
        /// </summary>
        public void Restart()
        {
            Log.Info("Configuration changed: Restarting CruiseControl.NET Server ");

            StopIntegrators();
            CreateIntegrators();
            StartIntegrators();
        }

        public void Start()
        {
            Log.Info("Starting CruiseControl.NET Server");
            monitor.Reset();
            StartIntegrators();
        }

        /// <summary>
        /// Start integrator for specified project. 
        /// </summary>
        public void Start(string project)
        {
            IProjectIntegrator integrator = GetIntegrator(project);
            integrator.Start();
        }

        /// <summary>
        /// Stop all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
        /// </summary>
        public void Stop()
        {
            Log.Info("Stopping CruiseControl.NET Server");
            StopIntegrators();
            monitor.Set();
        }


        /// <summary>
        /// Stop integrator for specified project. 
        /// </summary>
        public void Stop(string project)
        {
            IProjectIntegrator integrator = GetIntegrator(project);
            integrator.Stop();
        }

        // ToDo - this done TDD
        // ToDo - really delete working dir? What if SCM hasn't changed?
        public void UpdateProject(string projectName, string serializedProject)
        {
            Log.Info("Updating project - " + projectName);
            try
            {
                IConfiguration configuration = configurationService.Load();
                configuration.Projects[projectName].Purge(true, false, true);
                configuration.DeleteProject(projectName);
                Project project = projectSerializer.Deserialize(serializedProject);
                configuration.AddProject(project);
                project.Initialize();
                configurationService.Save(configuration);
            }
            catch (ApplicationException e)
            {
                Log.Warning(e);
                throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
            }
        }

        /// <summary>
        /// Block thread until all integrators to have been stopped or aborted.
        /// </summary>
        public void WaitForExit()
        {
            monitor.WaitOne();
        }

        public void WaitForExit(string projectName)
        {
            GetIntegrator(projectName).WaitForExit();
        }

        #endregion

        #region Private Methods

        private void AbortIntegrators()
        {
            foreach (IProjectIntegrator integrator in projectIntegrators)
            {
                integrator.Abort();
            }
            WaitForIntegratorsToExit();
        }

        private void CreateIntegrators()
        {
            IConfiguration configuration = configurationService.Load();
            projectIntegrators = projectIntegratorListFactory.CreateProjectIntegrators(configuration.Projects);

            if (projectIntegrators.Count == 0)
            {
                Log.Info("No projects found");
            }
        }

        private IProjectIntegrator GetIntegrator(string projectName)
        {
            IProjectIntegrator integrator = projectIntegrators[projectName];
            if (integrator == null)
            {
                throw new CruiseControlException("Specified project does not exist: " + projectName);
            }
            return integrator;
        }

		private Project GetProjectInstance(string projectName)
		{
			return (Project)this.GetIntegrator(projectName);
		}

        void IDisposable.Dispose()
        {
            if (disposed) return;
            disposed = true;
            Abort();
        }

        private void StartIntegrators()
        {
            foreach (IProjectIntegrator integrator in projectIntegrators)
            {
                integrator.Start();
            }
        }

        private void StopIntegrators()
        {
            foreach (IProjectIntegrator integrator in projectIntegrators)
            {
                integrator.Stop();
            }
            WaitForIntegratorsToExit();
        }

        private void WaitForIntegratorsToExit()
        {
            foreach (IProjectIntegrator integrator in projectIntegrators)
            {
                integrator.WaitForExit();
            }
        }

        #endregion


        #region ICruiseServer Members


        public string GetBuildLogDirectory(string projectName)
        {
            IProjectIntegrator integrator = projectIntegrators[projectName];
            Project project = (Project)integrator.Project;

            XmlLogPublisher XmlPublisher = null;

            foreach (ITask CanidatePublisher in project.Publishers)
            {
                if (CanidatePublisher is XmlLogPublisher)
                {
                    XmlPublisher = (XmlLogPublisher)CanidatePublisher;
                    break;
                }
            }

            if (XmlPublisher == null)
                throw new InvalidOperationException("There is no xml log publisher!");

            return XmlPublisher.LogDirectory(project.ArtifactDirectory);
        }

        #endregion

        #region ICruiseServer Members


        public string GetHostServerName(string projectName)
        {
            return Environment.MachineName;
        }

        #endregion
    }
}