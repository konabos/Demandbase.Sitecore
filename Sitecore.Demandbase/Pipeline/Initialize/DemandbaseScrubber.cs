using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.Collections.Generic;
using System.Linq;

namespace SitecoreDemandbase.Pipeline.Initialize
{
    public class DemandbaseScrubber
    {
        internal static HashSet<ID> tracker = null;

        public void Clean()
        {
            using (new SecurityDisabler())
            {
                Clean(DemandbaseConstants.DefaultValuesFolderId);
                Clean(DemandbaseConstants.AccountWatchDefaultValuesFolderId);
                Clean(DemandbaseConstants.AccountWatchStringTypeFolder);
                Clean(DemandbaseConstants.AccountWatchIntTypeFolder);
                Clean(DemandbaseConstants.AccountWatchBoolTypeFolder);
                Clean(DemandbaseConstants.StringTypeFolder);
                Clean(DemandbaseConstants.IntTypeFolder);
                Clean(DemandbaseConstants.BoolTypeFolder);
                Clean(DemandbaseConstants.RuleElement);
            }
        }
        private void Clean(string id)
        {
            var db = Factory.GetDatabase("master", false);
            if (db == null) return;
            Item item = db.GetItem(id);
            foreach (Item child in item.Children.Where(x => !tracker.Contains(x.ID)))
                child.Delete();
        }
    }
}
