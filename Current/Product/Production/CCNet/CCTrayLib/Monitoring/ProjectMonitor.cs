using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
//using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection; //TODO -- Delete
//using ThoughtWorks.CruiseControl.Core; //TODO -- Delete
//using System.IO;  //TODO -- Delete

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{	
	public class ProjectMonitor : IProjectMonitor, ISingleProjectDetail
	{
        private ICruiseProjectManager cruiseProjectManager;
        private ProjectStatus lastProjectStatus;
		private Exception connectException;
		private BuildDurationTracker buildDurationTracker = new BuildDurationTracker();

        public ProjectMonitor(ICruiseProjectManager cruiseProjectManager)
        {
            this.cruiseProjectManager = cruiseProjectManager;        
		}

		// public for testing only
		public ProjectStatus ProjectStatus
		{
			get { return lastProjectStatus; }
		}

		public bool IsConnected
		{
			get { return lastProjectStatus != null; }
		}

		public ProjectActivity Activity
		{
			get { return lastProjectStatus.Activity; }
		}

		public string LastBuildLabel
		{
			get { return lastProjectStatus.LastBuildLabel; }
		}

		public DateTime LastBuildTime
		{
			get { return lastProjectStatus.LastBuildDate; }
		}

		public DateTime NextBuildTime
		{
			get { return lastProjectStatus.NextBuildTime; }
		}

		public string WebURL
		{
			get { return lastProjectStatus.WebURL; }
		}

		public string ProjectName
		{
			get { return cruiseProjectManager.ProjectName; }
		}

		public Exception ConnectException
		{
			get { return connectException; }
		}

		public ProjectState ProjectState
		{
			get
			{
				// nb: deliberately copy project status variable for thread safety
				ProjectStatus status = ProjectStatus;

				if (status == null)
					return ProjectState.NotConnected;

				if (status.Activity == ProjectActivity.Building)
					return ProjectState.Building;

				if (status.BuildStatus == IntegrationStatus.Success)
					return ProjectState.Success;

				return ProjectState.Broken;
			}
		}

		public ISingleProjectDetail Detail
		{
			get { return this; }
		}

		public void ForceBuild()
		{
            cruiseProjectManager.ForceBuild();
		}

		public void Poll()
		{
			try
			{
				ProjectStatus newProjectStatus = cruiseProjectManager.ProjectStatus;
				if (lastProjectStatus != null && newProjectStatus != null)
				{
					CheckForBuildStart(lastProjectStatus, newProjectStatus);
					
					if (lastProjectStatus.LastBuildDate != newProjectStatus.LastBuildDate)
					{
						BuildTransition transition = CalculateBuildTransition(lastProjectStatus, newProjectStatus);
						
						CheckForSuccessfulBuild(transition);
						OnBuildOccurred(new MonitorBuildOccurredEventArgs(this, transition));
					}
				}
				lastProjectStatus = newProjectStatus;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Exception during poll: " + ex);
				lastProjectStatus = null;
				connectException = ex;
			}

			OnPolled(new MonitorPolledEventArgs(this));
		}

		private void CheckForSuccessfulBuild(BuildTransition transition)
		{
			if (transition == BuildTransition.Fixed || transition == BuildTransition.StillSuccessful)
			{
				buildDurationTracker.OnSuccessfulBuild();
			}
		}

		private void CheckForBuildStart(ProjectStatus lastProjectStatus, ProjectStatus newProjectStatus)
		{
			if (lastProjectStatus.Activity != ProjectActivity.Building && newProjectStatus.Activity == ProjectActivity.Building)
			{
				buildDurationTracker.OnBuildStart();
			}
		}


		public event MonitorBuildOccurredEventHandler BuildOccurred;
		public event MonitorPolledEventHandler Polled;

		protected void OnBuildOccurred(MonitorBuildOccurredEventArgs args)
		{
			if (BuildOccurred != null)
				BuildOccurred(this, args);
		}

		protected void OnPolled(MonitorPolledEventArgs args)
		{
			if (Polled != null)
				Polled(this, args);
		}

		private BuildTransition CalculateBuildTransition(ProjectStatus oldStatus, ProjectStatus newStatus)
		{
			bool wasOk = oldStatus.BuildStatus == IntegrationStatus.Success;
			bool isOk = newStatus.BuildStatus == IntegrationStatus.Success;

			if (wasOk && isOk)
				return BuildTransition.StillSuccessful;
			else if (!wasOk && !isOk)
				return BuildTransition.StillFailing;
			else if (wasOk && !isOk)
				return BuildTransition.Broken;
			else if (!wasOk && isOk)
				return BuildTransition.Fixed;

			throw new Exception("The universe has gone crazy.");
		}

		public string SummaryStatusString
		{
			get
			{
				ProjectState state = ProjectState;

				if (state == ProjectState.Success)
					return String.Empty;

				return ProjectName + ": " + state;
			}
		}
        
		public TimeSpan EstimatedTimeRemainingOnCurrentBuild
		{
			get { return buildDurationTracker.EstimatedTimeRemainingOnCurrentBuild; }
		}
	}
}