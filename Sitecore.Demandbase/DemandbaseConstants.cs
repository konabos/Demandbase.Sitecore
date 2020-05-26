using System;

namespace SitecoreDemandbase
{
    public class DemandbaseConstants
    {
        //Templates
        public const string ValueListTemplateId = "{009FC399-7977-44B2-B2AB-827AC1B64AF0}";
        public const string ValueTemplateId = "{0117897E-28D7-4932-9402-6B71175CEF11}";
        public const string AttributeTemplateId = "{8457FE74-B99C-4148-9A80-05358875778F}";
        public const string RuleCondition = "{F0D16EEE-3A05-4E43-A082-795A32B873C0}";
        //Folder locations
        public const string AttributesFolderId = "{B0E3BB19-1339-4827-8038-FAE3ED43A27D}";
        public const string DefaultValuesFolderId = "{D67C00C2-75A6-48B5-9DDB-769E6F6BEDFE}";
        public const string AccountWatchAttributesFolderId = "{F66D05A5-0432-46C3-8909-44EC80709CEB}";
        public const string AccountWatchDefaultValuesFolderId = "{79B52C93-5D65-435B-A0A3-08477B88E76F}";
        public const string RuleElement = "{062927F2-DF50-430D-A38A-B8C7624E7C01}";
        public const string LevelsFolderId = "{27D81862-4BC5-4EE7-AE05-D0DA7EED4D47}";
        public const string DemandbaseTag = "{7DA6339D-EFA7-43BF-B363-D67D327CDB01}";
        public const string ConditionalRenderingsRuleContextTags = "{B60C6A64-B367-48F0-86D9-7FA1C3D3B603}";
        //Type Folders
        public const string BoolTypeFolder = "{42D77CBC-F012-4669-98F6-188B6C0C1E67}";
        public const string IntTypeFolder = "{E0CBAD14-DE85-45C7-ACFC-DB1716B0BB22}";
        public const string StringTypeFolder = "{6903150E-B8D5-4298-A75E-3D4C3B05EB36}";
        public const string AccountWatchBoolTypeFolder = "{73B33F01-9F02-437B-8175-EC4DA8342E17}";
        public const string AccountWatchIntTypeFolder = "{640DD776-D420-4360-ADE0-D2D36C78F7DE}";
        public const string AccountWatchStringTypeFolder = "{78A98E78-0215-4031-A016-F275A8BCC123}";
        //Guid Generation key
        public static readonly Guid IdGenerationGuid = new Guid("{57BD076E-038E-4D44-A8D7-21CD6A198303}");
        //Operator Ids
        public const string StringEqualToOperatorId = "{10537C58-1684-4CAB-B4C0-40C10907CE31}";
        //Condition folder items to guard
        public class Core
        {
            //Panel
            public const string DemandbaseMockIpPanelId = "{59D4BC37-C6A4-4916-93AF-87F746149C33}";
        }
        public class GuardedConditions
        {
            public const string Tags = "{A63D3BB8-237A-4047-A774-AE37D0EAEF78}";
            public const string Visibility = "{4FFDAD7B-BD68-4F03-B998-94CDCC80ECAD}";
            public const string DefaultAccountWatch = "{39D0B0FC-7186-4D80-8728-36B864674DE2}";
        }
    }
}
