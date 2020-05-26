using Sitecore.Analytics.Model.Framework;
using SitecoreDemandbase.Data.Interface;
using System;

namespace SitecoreDemandbase.Data
{
    [Serializable]
    public class XdbFacetDemandbaseData : Facet, IXdbFacetDemandbaseData
    {
        private const string FieldName = "DemandBaseData";
        private const string IpFieldName = "DemandbaseIpData";
        public XdbFacetDemandbaseData()
        {
            base.EnsureAttribute<string>(FieldName);
            base.EnsureAttribute<string>(IpFieldName);
        }

        public string DemandBaseData
        {
            get
            {
                return base.GetAttribute<string>(FieldName);
            }
            set
            {
                base.SetAttribute<string>(FieldName, (string)value);
            }
        }

        public string Ip
        {
            get
            {
                return base.GetAttribute<string>(IpFieldName);
            }
            set
            {
                base.SetAttribute<string>(IpFieldName, (string)value);
            }
        }
    }
}
