using System.Collections.Generic;
namespace ThoughtWorks.CruiseControl.Core.Util
{
	public interface ITransformer
	{
		string Transform(string input, string transformerFileName, Dictionary<string, string> xslParams);
	}
}
