﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;
using SitecoreDemandbase.Data;
using SitecoreDemandbase.Data.Interface;

namespace SitecoreDemandbase.Pipeline.Initialize
{
	public class InitializeDemandbase
	{
		private IUserDataService UserService { get; set; }
		private AttributeGenerator attributeGenerator;
		private RuleGenerator ruleGenerator = new RuleGenerator();
		private LevelSynchronizer _levelSynchronizer = new LevelSynchronizer();
		private bool _autoGenerate;

		public InitializeDemandbase(string restApi, string key, string demandbaseIp, string autoGenerate, string loadBalancerForwardingVariable)
		{
			if (key == "Enter your demandbase key here")
			{
				Log.Error("Unable to start Demandbase due to a missing API Key", this);
				return;
			}
			_autoGenerate = autoGenerate.ToLower() == "true";
			attributeGenerator = new AttributeGenerator(_autoGenerate);
			DemandbaseContext._restApi = restApi;
			DemandbaseContext._key = key;
			DemandbaseContext._demandbaseIp = demandbaseIp;
			DemandbaseContext._loadBalancerForwardingVariable = loadBalancerForwardingVariable;

			if (!_autoGenerate) return;

			var db = Factory.GetDatabase("master", false);

			if (db == null) return;

			CheckConditionalRenderings(db);

			if (!_levelSynchronizer.SynchronizeLevels(db)) return;			
		}

		private void CheckConditionalRenderings(Database db)
		{
			Item crc = db.GetItem(DemandbaseConstants.ConditionalRenderingsRuleContextTags);
			if (!crc["Tags"].Contains(DemandbaseConstants.DemandbaseTag))
			{
				using (new SecurityDisabler())
				{
					crc.BeginVersionCheckEditing();
					if (string.IsNullOrWhiteSpace(crc["Tags"]))
						crc["Tags"] = DemandbaseConstants.DemandbaseTag;
					else
						crc["Tags"] += "|" + DemandbaseConstants.DemandbaseTag;
					crc.Editing.EndEdit();
				}
			}
		}
		public void Process(PipelineArgs args)
		{
			DemandbaseContext.UserServiceSingleton = UserService;
			if (!_autoGenerate) return;
			var db = Factory.GetDatabase("master", false);
			if (db == null) return;
			DemandbaseContext._levels =
				db.GetItem(DemandbaseConstants.LevelsFolderId)
					.Children.Select(x => new Tuple<string, string>(x.Name, x["Value"]))
					.ToList();
			ruleGenerator.ProcessAttributes();
		}

		public void BuildAccountWatch(XmlNode arg)
		{
			if (attributeGenerator == null)
				return;

			var db = Factory.GetDatabase("master", false);
			if (db == null) return;
			var attributes = arg.SelectNodes("attribute");
			attributeGenerator.BuildAccountWatchAttributes(db, attributes);
		}

		public void BuildRules(XmlNode arg)
		{
			if (attributeGenerator == null)
				return;

			var db = Factory.GetDatabase("master", false);
			if (db == null) return;
			var attributes = arg.SelectNodes("attribute");
			attributeGenerator.BuildAttributes(db, attributes);
		}

		private static float GetSitecoreVersion()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(HttpRuntime.AppDomainAppPath + "/sitecore/shell/sitecore.version.xml");
			var selectSingleNode = doc.SelectSingleNode("/information/version/major");
			float ret;
			int tmp;
			if (!int.TryParse(selectSingleNode?.InnerText, out tmp))
			{
				tmp = 8;
			}
			ret = tmp;
			selectSingleNode = doc.SelectSingleNode("/information/version/minor");
			if (!int.TryParse(selectSingleNode?.InnerText, out tmp))
			{
				tmp = 2;
			}
			ret += ((float)tmp / 10);
			return ret;
		}
	}
}
