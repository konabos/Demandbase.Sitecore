using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Analytics.Model.Framework;

namespace SitecoreDemandbase.Data.Interface
{
	public interface IXdbFacetDemandbaseData : IFacet
	{
		string DemandBaseData { get; set; }
		string Ip { get; set; }
	}
}
