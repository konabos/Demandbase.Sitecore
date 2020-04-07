using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SitecoreDemandbase.Rules.AccountWatch
{
	public class DefaultRule<T> : StringOperatorCondition<T>
		where T : RuleContext
	{
		public string AttributeValue { get; set; }
		public string StrValue { get; set; }

		protected override bool Execute(T ruleContext)
		{
			return Compare(DemandbaseContext.User.GetSecondTeirValue<string>("watch_list", AttributeValue) ?? "", StrValue ?? "");
		}
	}
}