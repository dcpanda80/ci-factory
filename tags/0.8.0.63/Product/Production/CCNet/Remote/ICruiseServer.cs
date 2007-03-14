using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	public interface ICruiseServer : IDisposable
	{
		/// <summary>
		/// Launches the CruiseControl.NET server and starts all project schedules it contains
		/// </summary>
		void Start();

		/// <summary>
		/// Requests all started projects within the CruiseControl.NET server to stop
		/// </summary>
		void Stop();

		/// <summary>
		/// Terminates the CruiseControl.NET server immediately, stopping all started projects
		/// </summary>
		void Abort();

		/// <summary>
		/// Wait for CruiseControl server to finish executing
		/// </summary>
		void WaitForExit();

		/// <summary>
		/// Retrieve CruiseManager interface for the server
		/// </summary>
		ICruiseManager CruiseManager { get; }

		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// for all projects on a cruise server
		/// </summary>
        ProjectStatus[] GetProjectStatus();
        ProjectStatus GetProjectStatus(string projectName);

		bool ForceBuild(string projectName, ForceFilterClientInfo[] clientInfo);

		void WaitForExit(string projectName);
		
		/// <summary>
		/// Returns the name of the most recent build for the specified project
		/// </summary>
		string GetLatestBuildName(string projectName);

		/// <summary>
		/// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
		/// </summary>
		string[] GetBuildNames(string projectName);

		/// <summary>
		/// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
		/// </summary>
		string[] GetMostRecentBuildNames(string projectName, int buildCount);

		/// <summary>
		/// Returns the build log contents for requested project and build name
		/// </summary>
		string GetLog(string projectName, string buildName);

		/// <summary>
		/// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
		/// </summary>
		string GetServerLog();

		/// <summary>
		/// Returns the version number of the server
		/// </summary>
		string GetVersion();

		/// <summary>
		/// Adds a project to the server
		/// </summary>
		void AddProject(string serializedProject);

		/// <summary>
		/// Deletes the specified project from the server
		/// </summary>
		void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);

		/// <summary>
		/// Returns the serialized form of the requested project from the server
		/// </summary>
		string GetProject(string name);

		/// <summary>
		/// Updates the specified project configuration on the server
		/// </summary>
		void UpdateProject(string projectName, string serializedProject);

		ExternalLink[] GetExternalLinks(string projectName);
	}
}