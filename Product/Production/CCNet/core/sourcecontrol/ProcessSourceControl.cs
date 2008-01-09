using System;
using System.Xml;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{

	public class Test
	{
		public void TestSerialization()
		{

			DefaultConfigurationFileLoader Loader = new DefaultConfigurationFileLoader();
			FileConfigurationService Service = new FileConfigurationService(Loader, null, new FileInfo(@"C:\Downloads\ccnet.config.xml"));
			
			IProject project = Service.Load().Projects["mh_refactor_p"];

			string Serialized = Serialize("project", project);
			System.Diagnostics.Debug.WriteLine(Serialized);
			Project Clone = (Project)Deserialize(Serialized);
		}

		public static string Serialize(string reflectorType, object subject)
		{
			StringWriter buffer = new StringWriter();
			new ReflectorTypeAttribute(reflectorType).Write(new XmlTextWriter(buffer), subject);
			return buffer.ToString();
		}

		public static object Deserialize(string serialized)
		{
			object Subject = NetReflector.Read(serialized);
			return Subject;
		}
	}

	public abstract class ProcessSourceControl : ISourceControl
	{
		protected ProcessExecutor executor;
		protected IHistoryParser historyParser;
		private	Timeout timeout = Timeout.DefaultTimeout;

		public ProcessSourceControl(IHistoryParser historyParser) : this(historyParser, new ProcessExecutor())
		{}

		public ProcessSourceControl(IHistoryParser historyParser, ProcessExecutor executor)
		{
			this.executor = executor;
			this.historyParser = historyParser;
		}

		[ReflectorProperty("timeout", InstanceType = typeof(Timeout), Required=false)]
		public Timeout Timeout
		{
			get
			{
				return timeout;
			}
			set
			{
				if (value==null) 
					timeout = Timeout.DefaultTimeout;
				else 
					timeout = value;
			}
			
		}

		public abstract Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to);

		public abstract void LabelSourceControl(IIntegrationResult result);

		protected Modification[] GetModifications(ProcessInfo info, DateTime from, DateTime to)
		{
			ProcessResult result = Execute(info);
			return ParseModifications(result, from, to);
		}

		protected ProcessResult Execute(ProcessInfo processInfo)
		{
			processInfo.TimeOut = Timeout.Millis;
			ProcessResult result = executor.Execute(processInfo);

			if (result.TimedOut)
			{
				throw new CruiseControlException("Source control operation has timed out.");
			}
			else if (result.Failed)
			{
				throw new CruiseControlException(string.Format("Source control operation failed: {0}. Process command: {1} {2}",
				                                               result.StandardError, processInfo.FileName, processInfo.Arguments));
			}
			else if (result.HasErrorOutput)
			{
				Log.Warning(string.Format("Source control wrote output to stderr: {0}", result.StandardError));
			}
			return result;
		}

		protected Modification[] ParseModifications(ProcessResult result, DateTime from, DateTime to)
		{
			return ParseModifications(new StringReader(result.StandardOutput), from, to);
		}

		protected Modification[] ParseModifications(TextReader reader, DateTime from, DateTime to)
		{
			return historyParser.Parse(reader, from, to);
		}

		public virtual void GetSource(IIntegrationResult result)
		{}

		public virtual void Initialize(IProject project)
		{}

		public void Purge(IProject project)
		{}
	}
}