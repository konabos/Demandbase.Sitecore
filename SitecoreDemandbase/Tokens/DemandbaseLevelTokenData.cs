using System.Linq;
using TokenManager.Data.Interfaces;

namespace SitecoreDemandbase.Tokens
{
	public class DemandbaseLevelTokenData : ITokenData
	{
		private string options;
		public DemandbaseLevelTokenData(string name, string label)
		{
			if (!DemandbaseContext.Levels.Any())
				options = "<option value=\"default\">Default</option>";
			else
			{
				options =
					DemandbaseContext.Levels.Select(x => $"<option value={x.Item2}>{x.Item1}</option>")
						.Aggregate((x, running) => running + x);
			}
			Required = false;
			Name = name;
			Label = label;
		}
		
		public object GetValue(string value)
		{
			return value;
		}

		public string Name { get; set; }
		public string Label { get; set; }
		public bool Required { get; set; }

		public string AngularMarkup
		{
			get
			{
				return
					$@"
	<div class=""field-row {{{{field.class}}}}"">
        <span class=""field-label"">{{{{field.Label}}}} </span>
        <div class=""field-data"">
            <select ng-model=""token.data[field.Name]"" ><option value=""account_watch"">Account Watch</option>{options}</select>
        </div>
    </div>
";
			}
		}

		public dynamic Data { get; }
	}
}
