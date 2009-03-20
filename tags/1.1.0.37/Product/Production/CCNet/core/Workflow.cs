using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// A generic project contains a collection of tasks.  It will execute them in the specified order.  It is possible to have multiple tasks of the same type.
	/// <code>
	/// <![CDATA[
	/// <workflow name="foo">
	///		<tasks>
	///			<sourcecontrol type="cvs"></sourcecontrol>
	///			<build type="nant"></build>
	///		</tasks>
	///		<state type="state"></state>
	/// </workflow>
	/// ]]>
	/// </code>
	/// </summary>
	[ReflectorType("workflow")]
	public class Workflow : ProjectBase, IProject
	{
		private IList _tasks = new ArrayList();
		private WorkflowResult _currentIntegrationResult;

		public IIntegrationResultManager IntegrationResultManager
		{
			get
			{
				return null;
			}
		}

		[ReflectorCollection("tasks", InstanceType = typeof(ArrayList))]
		public IList Tasks
		{
			get { return _tasks; }
			set { _tasks = value; }
		}

		public IntegrationResult CurrentIntegration
		{
			get { return _currentIntegrationResult; }
		}

		public IIntegrationResult RunIntegration(IIntegrationResult result)
		{
			_currentIntegrationResult = new WorkflowResult();

			foreach (ITask task in Tasks)
			{
				try 
				{ 
					RunTask(task); 
				}
				catch (CruiseControlException ex) 
				{
					_currentIntegrationResult.ExceptionResult = ex;
				}
			}
			return _currentIntegrationResult;
		}

		private void RunTask(ITask task)
		{
			task.Run(_currentIntegrationResult);
		}
		
		public IntegrationStatus LatestBuildStatus
		{
			get { return _currentIntegrationResult.Status; }
		}

		public void Purge(bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			return;
		}

		public int MinimumSleepTimeMillis 
		{ 
			get { return 0; }
		}

		public ProjectActivity CurrentActivity 
		{
			get { return ProjectActivity.Sleeping; }
		}

		public string WebURL 
		{ 
			get { return ""; }
		}
	}
}
