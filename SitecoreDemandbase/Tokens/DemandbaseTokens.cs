using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreDemandbase.Data;
using TokenManager.Data.Interfaces;
using TokenManager.Data.TokenDataTypes;
using TokenManager.Data.TokenDataTypes.Support;
using TokenManager.Data.TokenExtensions;
using TokenManager.Data.Tokens;

namespace SitecoreDemandbase.Tokens
{
	public class DemandbaseTokens : AutoToken
	{
		public DemandbaseTokens() : base("Demandbase", "Applications/16x16/flash.png", "Attribute")
		{
		}

		public override string TokenIdentifierText(TokenDataCollection extraData)
		{
			DemandbaseAttribute attribute = DemandbaseContext.Attributes.Values.FirstOrDefault(x => x.Id == extraData["attribute"]);
			return $"Demandbase {extraData["level"]} {attribute?.Name ?? extraData["attribute"]}";
		}

		public override TokenButton TokenButton()
		{
			return new TokenButton()
			{
				Icon = this.TokenIcon,
				Name = "Insert a Demandbase attribute"
			};
		}

		public override string Value(TokenDataCollection extraData)
		{
			string ret = DemandbaseContext.User.GetSecondTeirValue<object>("watch_list", extraData["attribute"])?.ToString();
			if (ret != null) return ret;
			ret = DemandbaseContext.User.GetSecondTeirValue<object>(extraData["level"], extraData["attribute"])?.ToString();
			if (ret != null) return ret;
			return DemandbaseContext.User.GetValue<object>(extraData["attribute"])?.ToString() ?? "";
		}

		public override IEnumerable<ITokenData> ExtraData()
		{
			yield return new DemandbaseLevelTokenData("level", "Demandbase Level");
			yield return new DemandbaseTokenData("attribute", "Demandbase Attribute");
		}
	}
}
