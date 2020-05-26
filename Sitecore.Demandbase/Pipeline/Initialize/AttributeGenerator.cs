using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using SitecoreDemandbase.Data;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SitecoreDemandbase.Pipeline.Initialize
{
    public class AttributeGenerator
    {
        private bool _generate;
        public AttributeGenerator(bool generate)
        {
            _generate = generate;
        }
        public void BuildAccountWatchAttributes(Database db, XmlNodeList attributes)
        {
            Item attributesFolder = db.GetItem(DemandbaseConstants.AccountWatchAttributesFolderId);
            Item defaultValuesFolder = db.GetItem(DemandbaseConstants.AccountWatchDefaultValuesFolderId);
            foreach (var attribute in ProcessXmlAttributes(attributesFolder, attributes, db, defaultValuesFolder, true))
                DemandbaseContext.AccountWatch.Add(attribute.Name, attribute);
        }

        public void BuildAttributes(Database db, XmlNodeList attributes)
        {
            Item attributesFolder = db.GetItem(DemandbaseConstants.AttributesFolderId);
            Item defaultValuesFolder = db.GetItem(DemandbaseConstants.DefaultValuesFolderId);
            foreach (var attribute in ProcessXmlAttributes(attributesFolder, attributes, db, defaultValuesFolder, false))
                DemandbaseContext.Attributes.Add(attribute.Name, attribute);
        }
        private IEnumerable<DemandbaseAttribute> ProcessXmlAttributes(Item attributesFolder, XmlNodeList attributes, Database db, Item defaultValuesFolder, bool accountWatch)
        {
            using (new SecurityDisabler())
            {
                if (attributes != null)
                    foreach (XmlNode key in attributes)
                    {
                        var attribute = new DemandbaseAttribute(key, accountWatch);
                        yield return attribute;
                        if (!_generate)
                            continue;
                        Item itemAttribute = null;
                        if (attribute.Customizable || (!attribute.Customizable && !attribute.DefaultValues.Any()))
                        {
                            itemAttribute = db.GetItem(GuidUtility.GetId("demandbase" + FindParent(attribute, db).ID, attribute.Id));
                            if (itemAttribute == null || itemAttribute["Name"] != attribute.Name || itemAttribute["Id"] != attribute.Id)
                            {
                                itemAttribute = InstallAttribute(attribute, attributesFolder);
                            }
                            DemandbaseScrubber.tracker.Add(itemAttribute.ID);
                        }

                        Item valueFolder = null;
                        valueFolder = db.GetItem(GuidUtility.GetId("demandbasevaluefolder" + defaultValuesFolder.ID, attribute.Id));
                        if (valueFolder == null && attribute.DefaultValues.Any())
                        {
                            valueFolder = InstallValueFolder(attribute, defaultValuesFolder);
                        }
                        else if (valueFolder != null)
                        {
                            if (valueFolder["Attribute Id"] != attribute.Id)
                            {
                                valueFolder.BeginVersionCheckEditing();
                                valueFolder["Attribute Id"] = attribute.Id;
                                valueFolder.Editing.EndEdit();
                            }
                            foreach (Item valueItem in valueFolder.Children.Where(x => !attribute.DefaultValues.Contains(x["Value"])))
                                valueItem.Delete();
                            foreach (string value in attribute.DefaultValues)
                            {
                                Item valueItem = db.GetItem(GuidUtility.GetId("demandbasevalue" + defaultValuesFolder.ID + attribute.Id, value)) ??
                                                 InstallValue(attribute, value, valueFolder);
                                if (valueItem["Value"] == value) continue;
                                valueItem.BeginVersionCheckEditing();
                                valueItem["Value"] = value;
                                valueItem.Editing.EndEdit();
                            }
                        }
                        if (attribute.DefaultValues.Any())
                            DemandbaseScrubber.tracker.Add(valueFolder.ID);
                    }
            }
        }
        private Item InstallValue(DemandbaseAttribute attribute, string value, Item valueFolder)
        {
            Item ret = valueFolder.Database.DataManager.DataEngine.CreateItem(ItemUtil.ProposeValidItemName(value), valueFolder,
                new ID(DemandbaseConstants.ValueTemplateId), GuidUtility.GetId("demandbasevalue" + valueFolder.ID + attribute.Id, value));
            ret.BeginVersionCheckEditing();
            ret["Value"] = value;
            ret.Editing.EndEdit();
            return ret;
        }

        private Item InstallValueFolder(DemandbaseAttribute attribute, Item defaultValuesFolder)
        {
            Item folder = defaultValuesFolder.Database.DataManager.DataEngine.CreateItem(ItemUtil.ProposeValidItemName(attribute.Name),
                defaultValuesFolder, new ID(DemandbaseConstants.ValueListTemplateId),
                GuidUtility.GetId("demandbasevaluefolder" + defaultValuesFolder.ID, attribute.Id));
            folder.BeginVersionCheckEditing();
            folder["Attribute Id"] = attribute.Id;
            folder.Editing.EndEdit();
            foreach (string value in attribute.DefaultValues)
                InstallValue(attribute, value, folder);
            return folder;
        }

        private Item InstallAttribute(DemandbaseAttribute attribute, Item attributeFolder)
        {
            Item ret = attributeFolder.Database.DataManager.DataEngine.CreateItem(
                ItemUtil.ProposeValidItemName(attribute.Name),
                FindParent(attribute, attributeFolder.Database),
                new ID(DemandbaseConstants.AttributeTemplateId),
                GuidUtility.GetId("demandbase" + attributeFolder.ID, attribute.Id));
            ret.BeginVersionCheckEditing();
            ret["Name"] = attribute.Name;
            ret["Id"] = attribute.Id;
            ret.Editing.EndEdit();
            return ret;
        }

        public static Item FindParent(DemandbaseAttribute attribute, Database db)
        {
            switch (attribute.Type)
            {
                case "string":
                    return
                        db.GetItem(attribute.AccountWatch
                            ? DemandbaseConstants.AccountWatchStringTypeFolder
                            : DemandbaseConstants.StringTypeFolder);
                case "int":
                case "float":
                    return
                        db.GetItem(attribute.AccountWatch
                            ? DemandbaseConstants.AccountWatchIntTypeFolder
                            : DemandbaseConstants.IntTypeFolder);
                case "bool":
                    return
                        db.GetItem(attribute.AccountWatch
                            ? DemandbaseConstants.AccountWatchBoolTypeFolder
                            : DemandbaseConstants.BoolTypeFolder);
            }
            return null;
        }
    }
}
