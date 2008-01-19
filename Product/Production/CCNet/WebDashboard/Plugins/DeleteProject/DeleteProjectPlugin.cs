using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	// ToDo - Test!
	public class DeleteProjectPlugin : IPlugin
	{
		private readonly IActionInstantiator actionInstantiator;

		public DeleteProjectPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		public string LinkDescription
		{
			get { return "Delete Project"; }
		}

		public INamedAction[] NamedActions
		{
			get
			{
				return new INamedAction[]
				{
					new ImmutableNamedAction("ShowDeleteProject", actionInstantiator.InstantiateAction(typeof(ShowDeleteProjectAction))),
					new ImmutableNamedAction(DoDeleteProjectAction.ACTION_NAME, actionInstantiator.InstantiateAction(typeof(ShowDeleteProjectAction)))
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
