using System.Text;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Perforce
{
	public class P4ConfigProcessInfoCreator : IP4ProcessInfoCreator
	{
		public ProcessInfo CreateProcessInfo(P4 p4, string extraArguments)
		{
			ProcessInfo processInfo = new ProcessInfo(p4.Executable, BuildCommonArguments(p4) + extraArguments);
			processInfo.TimeOut = 0; // Don't time out - this should be configurable
			return processInfo;
		}

		private string BuildCommonArguments(P4 p4) 
		{
			StringBuilder args = new StringBuilder();
			args.Append("-s "); // for "scripting" mode
			if (p4.Client != null && p4.Client != string.Empty) 
			{
				args.Append("-c " + p4.Client + " ");
			}
			if (p4.Port != null && p4.Port != string.Empty) 
			{
				args.Append("-p " + p4.Port + " ");
			}
			if (p4.User != null && p4.Port != string.Empty)
			{
				args.Append("-u " + p4.User + " ");
			}
			return args.ToString();
		}
	}
}