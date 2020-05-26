using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;

namespace SitecoreDemandbase.Pipeline.Personalize
{
	public class PreviewPersonalize : Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering.Personalize
	{
		public override void Process(CustomizeRenderingArgs args)
		{
			Assert.ArgumentNotNull((object)args, "args");
			if (args.IsCustomized || !Context.PageMode.IsNormal)
				return;
			this.Evaluate(args);
		}
	}
}
