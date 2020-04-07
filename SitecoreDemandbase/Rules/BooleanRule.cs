using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SitecoreDemandbase.Rules
{
	public class BooleanRule<T> : StringOperatorCondition<T>
	where T : RuleContext
	{
		public ID RootValue { get; set; }
		public ID IdAttribute { get; set; }
		public string StrValue { get; set; }

		protected override bool Execute(T ruleContext)
		{
			string level = RootValue == (ID)null ? "" : Context.Database.GetItem(RootValue, LanguageManager.DefaultLanguage)?["Value"] ?? "";
			string attribute = Context.Database.GetItem(IdAttribute, LanguageManager.DefaultLanguage)["Id"];
			if (level == "")
				return DemandbaseContext.User.GetValue<bool>(attribute);
			return DemandbaseContext.User.GetSecondTeirValue<bool>(level, attribute);
		}
	}
}
