using System;
using Sitecore.Collections;
using Sitecore.ExperienceEditor.Speak.Ribbon;
using Sitecore.Shell.Web.UI.WebControls;

namespace SitecoreDemandbase.Commands
{
	public class MockIpPanel : CustomRibbonPanel
	{
		protected override SafeDictionary<Type, object> Panels => new SafeDictionary<Type, object>
		{
				  {
		typeof (RibbonComponentControlBase),
		(object) new MockIpBase()
	  }
		};
	}
}
