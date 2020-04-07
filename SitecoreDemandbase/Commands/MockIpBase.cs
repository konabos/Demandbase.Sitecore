using System.Web;
using System.Web.UI;
using Sitecore.ExperienceEditor.Speak.Ribbon;

namespace SitecoreDemandbase.Commands
{
	public class MockIpBase : RibbonComponentControlBase
	{
		protected override void Render(HtmlTextWriter output)
		{
			bool isPreview = (HttpContext.Current?.Request.QueryString["sc_mode"] ?? HttpContext.Current?.Request.QueryString["mode"]) == "preview";

			output.Write(
				@"
			<table id='demandbasePanelBase'><tr><td>
			<input id='demandbase-mockip' size='24' placeholder='Enter Simulation Ip Address' onKeyUp='window.top.document.cookie = ""demandbasemockip=""+this.value;'; />
			</td><td>
			<a style='height: 16px;margin-left: 4px;padding: 4px 4px;cursor:pointer;color:blue;' class='scButton' onclick='window.top.location.reload()'>Simulate</a>
			</td></tr><tr style=""padding:3px;color: darkred;font-weight: bold;""><td colspan=""2"" id=""ipsimulationoutput""></td></tr></table>
			<script>
			var demandbasemockip = document.getElementById('demandbase-mockip');
			demandbasemockip.value = window.top.document.cookie.replace(/(?:(?:^|.*;\s*)demandbasemockip\s*\=\s*([^;]*).*$)|^.*$/, '$1');
			if (demandbasemockip.value !== """"){
				document.getElementById(""ipsimulationoutput"").innerHTML = ""Currently simulating IP: ""+demandbasemockip.value;
			}
			</script>
			");
			if (!isPreview)
			{
				output.Write("<script>document.getElementById('demandbasePanelBase').parentNode.parentNode.parentNode.style.display = 'none';</script>");
			}
		}
	}
}
