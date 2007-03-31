namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	/// <summary>
	/// Used to (at least) wrap HttpContext.Server.MapPath
	/// </summary>
	public interface IPathMapper
	{
		string GetLocalPathFromURLPath(string originalPath);
		string PhysicalApplicationPath { get; }
	}
}
