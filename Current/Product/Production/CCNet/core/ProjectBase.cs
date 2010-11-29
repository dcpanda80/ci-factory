using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.IntegrationFilters;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public abstract class ProjectBase
	{
		public static readonly string DefaultWorkingSubDirectory = "WorkingDirectory";
		public static readonly string DefaultArtifactSubDirectory = "Artifacts";

		private string name;
		private string configuredWorkingDirectory = "";
		private string configuredArtifactDirectory = "";
		private ITrigger[] triggers = new ITrigger[] { new IntervalTrigger() };
		private ExternalLink[] externalLinks = new ExternalLink[0];
		private IForceFilter[] forceFilters;
		private IntegrationFilter integrationFilter;
        private ArrayList killExceptions = new ArrayList();
		
		[ReflectorProperty("integrationFilter", InstanceType = typeof(IntegrationFilter), Required=true)]
		public IntegrationFilter IntegrationFilter
		{
			get
			{
				return integrationFilter;
			}
			set
			{
				integrationFilter = value;
			}
		}

		[ReflectorArray("forceFilters", Required=false)]
		public IForceFilter[] ForceFilters
		{
			get
			{
				return forceFilters;
			}
			set
			{
				forceFilters = value;
			}
		}

		[ReflectorProperty("name")]
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ReflectorArray("triggers", Required=false)]
		public ITrigger[] Triggers
		{
			get { return triggers; }
			set { triggers = value; }
		}

		[ReflectorProperty("workingDirectory", Required=false)]
		public string ConfiguredWorkingDirectory
		{
			get { return configuredWorkingDirectory; }
			set { configuredWorkingDirectory = value; }
		}

		[ReflectorProperty("artifactDirectory", Required=false)]
		public string ConfiguredArtifactDirectory
		{
			get { return configuredArtifactDirectory; }
			set { configuredArtifactDirectory = value; }
		}

		[ReflectorArray("externalLinks", Required=false)]
		public ExternalLink[] ExternalLinks
		{
			get { return externalLinks; }
			set { externalLinks = value; }
		}

		public string WorkingDirectory
		{
			get
			{
				if (StringUtil.IsBlank(configuredWorkingDirectory))
				{
					return new DirectoryInfo(Path.Combine(Name, DefaultWorkingSubDirectory)).FullName;
				}
				return new DirectoryInfo(configuredWorkingDirectory).FullName;
			}
		}

        [ReflectorProperty("killExceptions", Required = false)]
        public ArrayList KillExceptions
        {
            get { return killExceptions; }
            set { killExceptions = value; }
        }

		public string ArtifactDirectory
		{
			get
			{
				if (StringUtil.IsBlank(configuredArtifactDirectory))
				{
					return new DirectoryInfo(Path.Combine(Name, DefaultArtifactSubDirectory)).FullName;
				}
				return new DirectoryInfo(configuredArtifactDirectory).FullName;
			}
		}
	}
}