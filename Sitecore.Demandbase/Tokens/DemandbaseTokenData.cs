using System.Linq;
using TokenManager.Data.Interfaces;

namespace SitecoreDemandbase.Tokens
{
    public class DemandbaseTokenData : ITokenData
    {
        private string options;
        public DemandbaseTokenData(string name, string label)
        {
            if (!DemandbaseContext.Attributes.Any())
                options = "";
            else
                options = DemandbaseContext.Attributes.Values.Select(x => $"<option value={x.Id}>{x.Name}</option>")
                    .Aggregate((x, running) => running + x);
            Required = true;
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
            <select ng-if=""token.data['level'] !== 'account_watch'"" ng-model=""token.data[field.Name]"" >{options}</select>
			<input ng-if=""token.data['level'] === 'account_watch'"" ng-model=""token.data[field.Name]"" placeholder=""Watch list attribute id"" />
        </div>
    </div>
";
            }
        }

        public dynamic Data { get; }
    }
}
