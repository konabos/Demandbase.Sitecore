using Sitecore.Data.Items;

namespace SitecoreDemandbase
{
	public static class ItemEditingExtensions
	{
		public static void BeginVersionCheckEditing(this Item item)
		{
			if (item.Versions.Count == 0)
				item.Versions.AddVersion();
			item.Editing.BeginEdit();
		}
	}
}
