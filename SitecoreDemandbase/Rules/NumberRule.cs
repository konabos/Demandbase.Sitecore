using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SitecoreDemandbase.Rules
{
	public class NumberRule<T> : OperatorCondition<T>
		where T : RuleContext
	{
		public ID RootValue { get; set; }
		public ID IdAttribute { get; set; }
		public int IntValue { get; set; }

		protected override bool Execute(T ruleContext)
		{
			string level = RootValue == (ID)null ? "" : Context.Database.GetItem(RootValue, LanguageManager.DefaultLanguage)?["Value"] ?? "";
			string attribute = null;
			attribute = Context.Database.GetItem(IdAttribute, LanguageManager.DefaultLanguage)["Id"];
			if (level == "")
				return Compare(DemandbaseContext.User.GetValue<double>(attribute), IntValue);
			return Compare(DemandbaseContext.User.GetSecondTeirValue<double>(level, attribute), IntValue);
		}
		protected bool Compare(double value, double value2)
		{
			switch (this.GetOperator())
			{
				case ConditionOperator.Equal:
					return value == value2;
				case ConditionOperator.GreaterThanOrEqual:
					return value >= value2;
				case ConditionOperator.GreaterThan:
					return value > value2;
				case ConditionOperator.LessThanOrEqual:
					return value <= value2;
				case ConditionOperator.LessThan:
					return value < value2;
				case ConditionOperator.NotEqual:
					return value != value2;
				default:
					return value == value2;
			}
		}
	}
}