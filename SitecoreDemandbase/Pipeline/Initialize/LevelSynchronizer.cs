using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;

namespace SitecoreDemandbase.Pipeline.Initialize
{
	public class LevelSynchronizer
	{
		public const string DomesticHq = "{77180102-78E5-4927-A7DF-8EF8B3BCE0BA}";
		public const string Hq = "{32E3F5F8-638B-4442-9F30-7758F08DB41E}";
		public const string WorldHq = "{36EBC7C0-2BB9-407C-8439-1EECF37AB5EF}";
		public const string DemandbaseValueTemplate = "{0117897E-28D7-4932-9402-6B71175CEF11}";
		public const string LevelsItem = "{27D81862-4BC5-4EE7-AE05-D0DA7EED4D47}";
		public bool SynchronizeLevels(Database db)
		{
			HttpClient wc = new HttpClient();
			try
			{
				string str =
					wc.GetStringAsync($"{DemandbaseContext.RestApi}?query={DemandbaseContext.DemandbaseIp}&key={DemandbaseContext.Key}")
						.Result;
				var data = new HashSet<string>(
					JsonNetWrapper.DeserializeObject<IDictionary<string, object>>(str)
						.Where(x => x.Value?.GetType().FullName == "Newtonsoft.Json.Linq.JObject")
						.Select(x => x.Key));
				using (new SecurityDisabler())
				{
					if (data.Contains("domestichq"))
						ValidateInstalled(db, DomesticHq, "Domestic HQ", "domestichq");
					else
						ValidateRemoved(db, DomesticHq);
					if (data.Contains("hq"))
						ValidateInstalled(db, Hq, "HQ", "hq");
					else
						ValidateRemoved(db, Hq);
					if (data.Contains("worldhq"))
						ValidateInstalled(db, WorldHq, "World HQ", "worldhq");
					else
						ValidateRemoved(db, WorldHq);
				}
				if (data.Count <= 1)
					DemandbaseContext._noHq = true;
				return true;
			}
			catch (Exception e)
			{
				Log.Error("Unable to synchronize levels, this is likely due to the Demandbase service being down.  Aborting validation of rules as without proper levels it could have a detrimental effect on existing rules.", e, this);
			}
			return false;
		}

		private void ValidateRemoved(Database db, string id)
		{
			db.GetItem(id)?.Delete();
		}

		private void ValidateInstalled(Database db, string id, string name, string key)
		{
			Item item = db.GetItem(id);
			if (item == null)
				item = db.Engines.DataEngine.CreateItem(name, db.GetItem(LevelsItem), new ID(DemandbaseValueTemplate), new ID(id));
			else if (item["Value"] == key && item[FieldIDs.DisplayName] == name)
				return;
			item.BeginVersionCheckEditing();
			item["Value"] = key;
			item[FieldIDs.DisplayName] = name;
			item.Editing.EndEdit();
		}
	}
}
