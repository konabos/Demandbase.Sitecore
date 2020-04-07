using System.Web;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Web.UI.WebControls;
using Sitecore.Web.UI.WebControls.Ribbons;

namespace SitecoreDemandbase.Commands
{
	public class MockIp : RibbonPanel
	{
		public override void Render(HtmlTextWriter output, Ribbon ribbon, Item button, CommandContext context)
		{
			bool isPreview = (HttpContext.Current?.Request.QueryString["sc_mode"] ?? HttpContext.Current?.Request.QueryString["mode"]) == "preview";

			if (isPreview)
			{
				output.Write(
					@"
<table><tr><td>
<input id='demandbase-mockip' size='21' placeholder='Enter Simulation Ip Address' onKeyUp='window.top.document.cookie = ""demandbasemockip=""+this.value;'; />
</td><td>
<a style='height: 16px;margin-left: 4px;padding: 4px 4px;cursor:pointer;color:blue;' class='scButton' onclick='window.top.location.reload()'>Simulate</a>
</td></tr><tr style=""padding:3px;color: darkred;font-weight: bold;""><td colspan=""2"" id=""ipsimulationoutput""></td></tr></table>
<script>
var demandbasemockip = document.getElementById('demandbase-mockip');
document.getElementById('RibbonForm').addEventListener('submit', function(ev) { ev.preventDefault(); });
demandbasemockip.value = window.top.document.cookie.replace(/(?:(?:^|.*;\s*)demandbasemockip\s*\=\s*([^;]*).*$)|^.*$/, '$1');
if (demandbasemockip.value !== """"){
	document.getElementById(""ipsimulationoutput"").innerHTML = ""Currently simulating IP: ""+demandbasemockip.value;
}</script>
");
			}
		}
	}
}
