using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines.GetLookupSourceItems;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;
using SitecoreDemandbase.Data;

namespace SitecoreDemandbase.Pipeline.Initialize
{
	public class RuleGenerator
	{
		private string DefaultConditionWording = "When {0}{2}{1}.";
		private string DefaultTreeCoding = " is equal to [IdValue,Tree,root={0},{1}]";
		private string DefaultStringCoding = " [operatorid,StringOperator,,compares to] [StrValue,,,String Value]";
		private string DefaultNumberCoding = " [operatorid,Operator,,compares to] [IntValue,Integer,,Number]";
		private string DefaultBooleanCoding = " is true";
		private string DefaultLevelCoding = $" in [RootValue,Tree,root={DemandbaseConstants.LevelsFolderId},Headquarter Hierarchy]";
		private string DefaultAttributeSelector = "[IdAttribute,Tree,root={0},Company Attribute]";
		private const string DefaultType = "SitecoreDemandbase.Rules.{0}{1}, SitecoreDemandbase";


		public RuleGenerator()
		{
			var db = Factory.GetDatabase("master", false);
			if (db == null) return;
			DemandbaseScrubber.tracker = new HashSet<ID>(typeof (DemandbaseConstants.GuardedConditions)
				.GetFields(BindingFlags.Static | BindingFlags.Public)
				.Select(f => new ID(f.GetValue(null).ToString())));
		}

		public void ProcessAttributes()
		{
			var db = Factory.GetDatabase("master", false);
			if (db == null) return;
			Item ruleElement = db.GetItem(DemandbaseConstants.RuleElement);
			using (new SecurityDisabler())
			{
				foreach (DemandbaseAttribute attribute in DemandbaseContext.Attributes.Values.Where(x => x.DefaultValues.Any()))
					ProcessAttributes(db, attribute, ruleElement, false);
				foreach (DemandbaseAttribute attribute in DemandbaseContext.AccountWatch.Values.Where(x => x.DefaultValues.Any()))
					ProcessAttributes(db, attribute, ruleElement, true);
				foreach (DemandbaseAttribute attribute in DemandbaseContext.AccountWatch.Values.Where(x => !x.DefaultValues.Any()))
					ProcessAttributes(db, attribute, ruleElement, true);
				ProcessDefault(db, "string", "Default String Type Condition", ruleElement, DemandbaseConstants.StringTypeFolder, false);
				ProcessDefault(db, "int", "Default Integer Type Condition", ruleElement, DemandbaseConstants.IntTypeFolder, false);
				ProcessDefault(db, "float", "Default Integer Type Condition", ruleElement, DemandbaseConstants.IntTypeFolder, false);
				ProcessDefault(db, "bool", "Default Boolean Type Condition", ruleElement, DemandbaseConstants.BoolTypeFolder, false);
			}
		}

		private void ProcessDefault(Database db, string type, string name, Item ruleElement, string attributeFolder, bool accountWatch)
		{
			Item attributeFolderItem = db.GetItem(attributeFolder);
			if (attributeFolderItem == null || !attributeFolderItem.Children.Any())
				return;
			Item condition = db.GetItem(GuidUtility.GetId("demandbaseconditiondefault", name + accountWatch));
			if (condition == null)
				condition = InstallCondition(ruleElement, type, accountWatch, name, attributeFolder, type);
			else
				SetCondition($"{(accountWatch ? "Watch List ": "")} " + DefaultAttributeSelector.FormatWith(attributeFolder), type, condition, null, accountWatch, false, "25");
			DemandbaseScrubber.tracker.Add(condition.ID);
		}

		private void ProcessAttributes(Database db, DemandbaseAttribute attribute, Item ruleElement, bool accountWatch)
		{
			string valueFolderId = "";
			if (attribute.DefaultValues.Any())
				valueFolderId = accountWatch
				? GuidUtility.GetId("demandbasevaluefolder" + DemandbaseConstants.AccountWatchDefaultValuesFolderId, attribute.Id).ToString()
				: GuidUtility.GetId("demandbasevaluefolder" + DemandbaseConstants.DefaultValuesFolderId, attribute.Id).ToString();
			Item condition = db.GetItem(GuidUtility.GetId("demandbasecondition", attribute.Id));
			if (condition == null)
				condition = InstallCondition(attribute, ruleElement, accountWatch, accountWatch ? DemandbaseConstants.AccountWatchAttributesFolderId : DemandbaseConstants.AttributesFolderId, valueFolderId);
			else
				SetCondition(attribute.Name, attribute.Type, condition, valueFolderId, accountWatch);
			DemandbaseScrubber.tracker.Add(condition.ID);
		}
		private Item InstallCondition(DemandbaseAttribute attribute, Item ruleElement, bool accountWatch, string attributeFolderId, string valuesFolderId)
		{
			Item condition = ruleElement.Database.DataManager.DataEngine.CreateItem(ItemUtil.ProposeValidItemName(attribute.Name),
				ruleElement, new ID(DemandbaseConstants.RuleCondition), GuidUtility.GetId("demandbasecondition" + attributeFolderId, attribute.Id));
			SetCondition(attribute.Name, attribute.Type, condition, valuesFolderId, accountWatch, attribute.DefaultValues.Any());
			return condition;
		}

		private Item InstallCondition(Item ruleElement, string type, bool accountWatch, string name, string attributeFolder, string attributeType)
		{
			Item condition = ruleElement.Database.DataManager.DataEngine.CreateItem(ItemUtil.ProposeValidItemName(name),
				ruleElement, new ID(DemandbaseConstants.RuleCondition), GuidUtility.GetId("demandbaseconditiondefault", name+accountWatch));
			SetCondition($"{(accountWatch ? "Watch List " : "")} " + DefaultAttributeSelector.FormatWith(attributeFolder), attributeType, condition, null, accountWatch, false, "25");
			return condition;
		}
		private void SetCondition(string attributeLabel, string attributeType, Item condition, string valuesFolderId, bool accountWatch, bool hasValues = false, string sortOrder = "")
		{
			string text = null;
			string type = null;
			if (hasValues)
				text = DefaultConditionWording.FormatWith("Default " +attributeLabel,
					DefaultTreeCoding.FormatWith(valuesFolderId, attributeLabel),
					accountWatch ? " in Watch List" : DemandbaseContext.NoHq ? "" : DefaultLevelCoding);
			switch (attributeType)
			{
				case "int":
				case "float":
					if (text == null)
						text = DefaultConditionWording.FormatWith(attributeLabel,
							DefaultNumberCoding, accountWatch || DemandbaseContext.NoHq ? "" : DefaultLevelCoding);
					type = DefaultType.FormatWith(accountWatch ? "AccountWatch." : "", "NumberRule");
					break;
				case "bool":
					if (text == null)
						text = DefaultConditionWording.FormatWith(attributeLabel,
							DefaultBooleanCoding, accountWatch || DemandbaseContext.NoHq ? "" : DefaultLevelCoding);
					type = DefaultType.FormatWith(accountWatch ? "AccountWatch." : "", "BooleanRule");
					break;
				default:
					if (text == null)
						text = DefaultConditionWording.FormatWith(attributeLabel,
							DefaultStringCoding, accountWatch || DemandbaseContext.NoHq ? "" : DefaultLevelCoding);
					type = DefaultType.FormatWith(accountWatch ? "AccountWatch." : "", "StringRule");
					break;
			}

			if (condition["Text"] != text || condition["Type"] != type || condition[FieldIDs.Sortorder] != sortOrder)
			{
				condition.BeginVersionCheckEditing();
				condition["Text"] = text;
				condition["Type"] = type;
				condition[FieldIDs.Sortorder] = sortOrder;
				condition.Editing.EndEdit();
			}
		}
	}
}
