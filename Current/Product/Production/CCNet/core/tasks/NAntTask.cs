using System;
using System.Collections;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	[ReflectorType("nant")]
	public class NAntTask : ITask
	{
		public const int DefaultBuildTimeout = 600;
		public const string DefaultExecutable = "nant.exe";
		public const string DefaultLogger = "NAnt.Core.XmlLogger";
		public const bool DefaultNoLogo = true;

		private ProcessExecutor executor;

		public NAntTask() : this(new ProcessExecutor())
		{
		}

		public NAntTask(ProcessExecutor executor)
		{
			this.executor = executor;
		}

		[ReflectorArray("targetList", Required = false)]
		public string[] Targets = new string[0];

		[ReflectorProperty("executable", Required = false)] 
		public string Executable = DefaultExecutable;

		[ReflectorProperty("buildFile", Required = false)] 
		public string BuildFile = string.Empty;

		[ReflectorProperty("baseDirectory", Required = false)]
		public string ConfiguredBaseDirectory = string.Empty;

		[ReflectorProperty("buildArgs", Required = false)]
		public string BuildArgs = string.Empty;

		[ReflectorProperty("logger", Required = false)]
		public string Logger = DefaultLogger;

		[ReflectorProperty("nologo", Required = false)]
		public bool NoLogo = DefaultNoLogo;

		/// <summary>
		/// Gets and sets the maximum number of seconds that the build may take.  If the build process takes longer than
		/// this period, it will be killed.  Specify this value as zero to disable process timeouts.
		/// </summary>
		[ReflectorProperty("buildTimeoutSeconds", Required = false)] 
		public int BuildTimeoutSeconds = DefaultBuildTimeout;

		/// <summary>
		/// Runs the integration using NAnt.  The build number is provided for labelling, build
		/// timeouts are enforced.  The specified targets are used for the specified NAnt build file.
		/// StdOut from nant.exe is redirected and stored.
		/// </summary>
		/// <param name="result">For storing build output.</param>
		public void Run(IIntegrationResult result)
		{
			ProcessResult processResult = AttemptExecute(CreateProcessInfo(result), result.ProjectName);
			result.AddTaskResult(new ProcessTaskResult(processResult));

			// is this right?? or should this break the build
			if (processResult.TimedOut)
			{
				throw new BuilderException(this, "NAnt process timed out (after " + BuildTimeoutSeconds + " seconds)");
			}
		}

		private ProcessInfo CreateProcessInfo(IIntegrationResult result)
		{
			ProcessInfo info = new ProcessInfo(Executable, CreateArgs(result), BaseDirectory(result));
			info.TimeOut = BuildTimeoutSeconds*1000;
			return info;
		}

		private string BaseDirectory(IIntegrationResult result)
		{
			return result.BaseFromWorkingDirectory(ConfiguredBaseDirectory);
		}

        protected ProcessResult AttemptExecute(ProcessInfo info, string projectName)
		{
			try
			{
				return executor.Execute(info, projectName);
			}
			catch (Exception e)
			{
				throw new BuilderException(this, string.Format("Unable to execute: {0}\n{1}", BuildCommand, e), e);
			}
		}

		private string BuildCommand
		{
			get { return string.Format("{0} {1}", Executable, BuildArgs); }
		}

		private string CreateArgs(IIntegrationResult result)
		{
			ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
			AppendNoLogoArg(buffer);
			AppendBuildFileArg(buffer);
			AppendLoggerArg(buffer);
			buffer.AppendArgument(BuildArgs);
			AppendIntegrationResultProperties(buffer, result);
			AppendTargets(buffer);
			return buffer.ToString();
		}

		private void AppendNoLogoArg(ProcessArgumentBuilder buffer)
		{
			buffer.AppendIf(NoLogo, "-nologo");
		}

		private void AppendBuildFileArg(ProcessArgumentBuilder buffer)
		{
			buffer.AppendArgument(@"-buildfile:{0}", SurroundInQuotesIfContainsSpace(BuildFile));
		}

		private void AppendLoggerArg(ProcessArgumentBuilder buffer)
		{
			buffer.AppendArgument("-logger:{0}", Logger);
		}

		private void AppendIntegrationResultProperties(ProcessArgumentBuilder buffer, IIntegrationResult result)
		{
			foreach (string key in result.IntegrationProperties.Keys)
			{
				object value = result.IntegrationProperties[key];
                if (value != null && !StringUtil.IsBlank(value.ToString()))
					buffer.AppendArgument(string.Format("-D:{0}={1}", key, SurroundInQuotesIfContainsSpace(RemoveTrailingSlash(value.ToString()))));
			}
		}

		private string RemoveTrailingSlash(string directory)
		{			
			return StringUtil.IsBlank(directory) ? string.Empty : directory.TrimEnd(Path.DirectorySeparatorChar);
		}

		private void AppendTargets(ProcessArgumentBuilder buffer)
		{
			for (int i = 0; i < Targets.Length; i++)
 			{
				buffer.AppendArgument(Targets[i]);
 			}
		}

		private string SurroundInQuotesIfContainsSpace(string value)
		{
			if (! StringUtil.IsBlank(value) && value.IndexOf(' ') >= 0)
				return string.Format(@"""{0}""", value);
			return value;
		}

		public override string ToString()
		{
			string baseDirectory = ConfiguredBaseDirectory != null ? ConfiguredBaseDirectory : "";
			return string.Format(@" BaseDirectory: {0}, Targets: {1}, Executable: {2}, BuildFile: {3}", baseDirectory, string.Join(", ", Targets), Executable, BuildFile);
		}

		public string TargetsForPresentation
		{
			get
			{
				StringBuilder combined = new StringBuilder();
				foreach (string file in Targets)
				{
					if (combined.Length > 0) combined.Append(Environment.NewLine);
					combined.Append(file);
				}
				return combined.ToString();
			}
			set
			{
				if (StringUtil.IsBlank(value))
				{
					Targets = new string[0];
					return;
				}
				ArrayList targets = new ArrayList();
				using (StringReader reader = new StringReader(value))
				{
					while (reader.Peek() >= 0)
					{
						targets.Add(reader.ReadLine());
					}
				}
				Targets = (string[]) targets.ToArray(typeof (string));
			}
		}
	}
}