using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SitecoreDemandbase.Rules.AccountWatch
{
	public class BooleanRule<T> : StringOperatorCondition<T>
		where T : RuleContext
	{
		public ID AttributeValue { get; set; }

		protected override bool Execute(T ruleContext)
		{
			string attribute = Context.Database.GetItem(AttributeValue, LanguageManager.DefaultLanguage)["Id"];
			return DemandbaseContext.User.GetSecondTeirValue<bool>("watch_list", attribute);
		}
	}
}