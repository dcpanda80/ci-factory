using System.Collections.Generic;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	// ToDo - something more clever with error cases?
	public class HtmlAwareMultiTransformer : IMultiTransformer
	{
		private readonly ITransformer delegateTransformer;

		public HtmlAwareMultiTransformer(ITransformer delegateTransformer)
		{
			this.delegateTransformer = delegateTransformer;
		}

		public string Transform(string input, string[] transformerFileNames, Dictionary<string, string> xslParams)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string transformerFileName in transformerFileNames)
			{
                builder.Append(delegateTransformer.Transform(input, transformerFileName, xslParams));
				builder.Append("<br/>");
			}
			return builder.ToString();
		}
	}
}
