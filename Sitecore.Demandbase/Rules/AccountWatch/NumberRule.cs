using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SitecoreDemandbase.Rules.AccountWatch
{
    public class IntRule<T> : OperatorCondition<T>
        where T : RuleContext
    {
        public ID IdAttribute { get; set; }
        public int IntValue { get; set; }

        protected override bool Execute(T ruleContext)
        {
            string attribute = null;
            attribute = Context.Database.GetItem(IdAttribute, LanguageManager.DefaultLanguage)["Id"];
            return Compare(DemandbaseContext.User.GetSecondTeirValue<float>("watch_list", attribute), IntValue);
        }
        protected bool Compare(float value, float value2)
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