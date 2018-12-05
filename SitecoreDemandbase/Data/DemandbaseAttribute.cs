using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SitecoreDemandbase.Data
{
	public class DemandbaseAttribute
	{
		public string Name { get; }
		public string Id { get; }
		public string Type { get; }
		public bool AccountWatch { get; }
		public bool Customizable { get; }
		public HashSet<string> DefaultValues { get; }

		public DemandbaseAttribute(XmlNode node, bool accountWatch)
		{
			AccountWatch = accountWatch;
			Name = node.SelectSingleNode("name")?.InnerText;
			Id = node.SelectSingleNode("id")?.InnerText;
			Type = node.SelectSingleNode("type")?.InnerText;
			Customizable = node.Attributes?["customizable"]?.InnerText == "1";
			DefaultValues = new HashSet<string>();
			var xmlNodeList = node.SelectSingleNode("values");
			var selectNodes = xmlNodeList?.SelectNodes("value");
			if (selectNodes != null)
				foreach (XmlNode value in selectNodes)
					DefaultValues.Add(value.InnerText);
		}

	}
}
