using System;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core 
{
	/// <summary>
	/// Exposes project management functionality (start, stop, status) via remoting.  
	/// The CCTray is one such example of an application that may make use of this remote interface.
	/// </summary>
	public class CruiseManager : MarshalByRefObject, ICruiseManager
	{
		private readonly ICruiseServer cruiseServer;

		public CruiseManager(ICruiseServer cruiseServer)
		{
			this.cruiseServer = cruiseServer;
		}

		public ProjectStatus[] GetProjectStatus()
		{
			return cruiseServer.GetProjectStatus();
        }

        public ProjectStatus GetProjectStatus(string projectName)
        {
            return cruiseServer.GetProjectStatus(projectName);
        }

		public bool ForceBuild(string project)
		{
			return this.ForceBuild(project, null);
		}
	
		public bool ForceBuild(string project, ForceFilterClientInfo[] clientInfo)
		{
			return cruiseServer.ForceBuild(project, clientInfo);
		}

		public void WaitForExit(string project)
		{
			cruiseServer.WaitForExit(project);
		}

		public string GetLatestBuildName(string projectName)
		{
			return cruiseServer.GetLatestBuildName(projectName);
		}

		public string[] GetBuildNames(string projectName)
		{
			return cruiseServer.GetBuildNames(projectName);
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			try
			{
				return cruiseServer.GetMostRecentBuildNames(projectName, buildCount);	
			}
			catch (Exception e)
			{
				Log.Error(e);
				throw new CruiseControlException("Unexpected exception caught on server", e);
			}
		}

		public string GetLog(string projectName, string buildName)
		{
			return cruiseServer.GetLog(projectName, buildName);
		}

		public string GetServerLog()
		{
			return cruiseServer.GetServerLog();
		}

		public void AddProject(string serializedProject)
		{
			cruiseServer.AddProject(serializedProject);
		}

		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			cruiseServer.DeleteProject(projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}

		public string GetProject(string projectName)
		{
			return cruiseServer.GetProject(projectName);
		}

		public void UpdateProject(string projectName, string serializedProject)
		{
			cruiseServer.UpdateProject(projectName, serializedProject);
		}

		public ExternalLink[] GetExternalLinks(string projectName)
		{
			return cruiseServer.GetExternalLinks(projectName);
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public string GetServerVersion()
		{
			return cruiseServer.GetVersion();
		}
	}
}