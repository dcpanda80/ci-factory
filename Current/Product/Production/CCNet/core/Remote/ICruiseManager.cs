
namespace ThoughtWorks.CruiseControl.Remote
{
	/// <remarks>
	/// Remote Interface to CruiseControl.NET.
	/// </remarks>
	public interface ICruiseManager
	{
		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// for all projects on a cruise server
		/// </summary>
		ProjectStatus [] GetProjectStatus();
        ProjectStatus GetProjectStatus(string projectName);
        ProjectStatus[] GetProjectStatusLite();
        ProjectStatus GetProjectStatusLite(string projectName);

        string[] GetProjectNames();

		/// <summary>
		/// Forces a build for the named project.
		/// </summary>
		/// <param name="projectName"></param>
		bool ForceBuild(string projectName);

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

        string GetBuildLogDirectory(string projectName);

        string GetHostServerName(string projectName);

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
		/// Returns the version of the server
		/// </summary>
		string GetServerVersion();

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
		string GetProject(string projectName);

		/// <summary>
		/// Updates the selected project on the server
		/// </summary>
		void UpdateProject(string projectName, string serializedProject);

		ExternalLink[] GetExternalLinks(string projectName);

        void Stop();
        void Start();
        void Stop(string projectName);
        void Start(string projectName);
        void Kill(string projectName);
	}
}