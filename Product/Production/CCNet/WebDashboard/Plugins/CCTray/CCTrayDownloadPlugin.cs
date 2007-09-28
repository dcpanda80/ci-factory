using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.CCTray
{
	[ReflectorType("cctrayDownloadPlugin")]
	public class CCTrayDownloadPlugin : IPlugin
	{
		private readonly IActionInstantiator actionInstantiator;

		public CCTrayDownloadPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		public string LinkDescription
		{
			get { return "Download CCTray"; }
		}

		public INamedAction[] NamedActions
		{
			get
			{
				return new INamedAction[]
					{
						new ImmutableNamedAction(CCTrayDownloadAction.ActionName, actionInstantiator.InstantiateAction(typeof (CCTrayDownloadAction)))
					};
			}
		}

        #region IPlugin Members


        string IPlugin.ImageFileName
        {
            get
            {
                throw new System.Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new System.Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}