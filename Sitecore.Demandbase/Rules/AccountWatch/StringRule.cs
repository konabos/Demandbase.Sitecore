using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SitecoreDemandbase.Rules.AccountWatch
{
    public class StringRule<T> : StringOperatorCondition<T>
        where T : RuleContext
    {
        public ID IdAttribute { get; set; }
        public string StrValue { get; set; }
        public ID IdValue { get; set; }


        protected override bool Execute(T ruleContext)
        {
            if (string.IsNullOrWhiteSpace(OperatorId))
                OperatorId = DemandbaseConstants.StringEqualToOperatorId;
            string attribute = null;
            string value = null;
            if (IdValue != default(ID))
            {
                Item valueItem = Context.Database.GetItem(IdValue, LanguageManager.DefaultLanguage);
                if (valueItem == null) return false;
                value = valueItem["Value"];
                attribute = Context.Database.GetItem(valueItem.Parent["Attribute"], LanguageManager.DefaultLanguage)["Id"];
            }
            else
            {
                attribute = Context.Database.GetItem(IdAttribute, LanguageManager.DefaultLanguage)["Id"];
                value = StrValue;
            }
            return Compare(DemandbaseContext.User.GetSecondTeirValue<string>("watch_list", attribute) ?? "", value ?? "");
        }
    }
}