using System;
using System.Text;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class AggregatingProjectMonitor : IProjectMonitor
	{
		private readonly IProjectMonitor[] monitors;

		public AggregatingProjectMonitor(params IProjectMonitor[] monitors)
		{
			this.monitors = monitors;

			foreach (IProjectMonitor monitor in this.monitors)
			{
				monitor.Polled += new MonitorPolledEventHandler(Monitor_Polled);
				monitor.BuildOccurred += new MonitorBuildOccurredEventHandler(Monitor_BuildOccurred);
			}
		}

		public void ForceBuild()
		{
			throw new NotImplementedException();
		}

		public string SummaryStatusString
		{
			get
			{
				StringBuilder result = new StringBuilder();
				bool firstOne = true;

				foreach (IProjectMonitor monitor in monitors)
				{
					string statusString = monitor.SummaryStatusString;
					if (statusString.Length == 0)
						continue;

					if (!firstOne)
						result.Append('\n');

					firstOne = false;

					result.Append(statusString);
				}

				if (result.Length == 0)
					return "All builds are good";

				return result.ToString();
			}
		}

		public event MonitorBuildOccurredEventHandler BuildOccurred;

		private void Monitor_BuildOccurred(object sender, MonitorBuildOccurredEventArgs e)
		{
			if (BuildOccurred != null)
			{
				BuildOccurred(this, e);
			}
		}

		public event MonitorPolledEventHandler Polled;

		private void Monitor_Polled(object sender, MonitorPolledEventArgs args)
		{
			if (Polled != null)
			{
				Polled(this, args);
			}
		}

		public void Poll()
		{
			foreach (IProjectMonitor monitor in monitors)
			{
				monitor.Poll();
			}
		}

		public ProjectState ProjectState
		{
			get
			{
				ProjectState worstState = ProjectState.Success;

				foreach (IProjectMonitor monitor in monitors)
				{
					if (monitor.ProjectState.IsMoreImportantThan(worstState))
						worstState = monitor.ProjectState;
				}

				return worstState;
			}
		}

		public ISingleProjectDetail Detail
		{
			get { throw new InvalidOperationException(); }
		}

	}
}