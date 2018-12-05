using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SitecoreDemandbase.Rules
{
	public class StringRule<T> : StringOperatorCondition<T>
	where T : RuleContext
	{
		public ID RootValue { get; set; }
		public ID IdAttribute { get; set; }
		public ID IdValue { get; set; }
		public string StrValue { get; set; }

		protected override bool Execute(T ruleContext)
		{
			if (string.IsNullOrWhiteSpace(OperatorId))
				OperatorId = DemandbaseConstants.StringEqualToOperatorId;
			string level = RootValue == (ID)null ? "" : Context.Database.GetItem(RootValue, LanguageManager.DefaultLanguage)?["Value"] ?? "";
			string attribute = null;
			string value = null;
			if (IdValue != default(ID))
			{
				Item valueItem = Context.Database.GetItem(IdValue, LanguageManager.DefaultLanguage);
				if (valueItem == null) return false;
				value = valueItem["Value"];
				attribute = valueItem.Parent["Attribute Id"];
			}
			else
			{
				attribute = Context.Database.GetItem(IdAttribute, LanguageManager.DefaultLanguage)["Id"];
				value = StrValue;
			}
			if (level == "")
				return Compare(DemandbaseContext.User.GetValue<string>(attribute) ?? "", value ?? "");
			return Compare(DemandbaseContext.User.GetSecondTeirValue<string>(level, attribute) ?? "", value ?? "");
		}
	}
}
