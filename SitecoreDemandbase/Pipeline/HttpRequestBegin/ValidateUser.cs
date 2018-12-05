using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Pipelines.HttpRequest;
using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace SitecoreDemandbase.Pipeline.HttpRequestBegin
{
	public class ValidateUser : HttpRequestProcessor
	{
		private static readonly Regex SubnetRegex =
			new Regex(@"(^127\.)|(^10\.)|(^172\.1[6-9]\.)|(^172\.2[0-9]\.)|(^172\.3[0-1]\.)|(^192\.168\.)|(^::1)",
				RegexOptions.Compiled);
		public override void Process(HttpRequestArgs args)
		{
			string mockIp = HttpContext.Current.Request.Cookies["demandbasemockip"]?.Value;
			if (!string.IsNullOrWhiteSpace(mockIp))
				RuleFactory.InvalidateCache();
		}
		/// <summary>
		/// from here http://stackoverflow.com/questions/735350/how-to-get-a-users-client-ip-address-in-asp-net
		/// </summary>
		/// <returns>users IP address</returns>
		public static string GetIpAddress()
		{
			var httpCookie = HttpContext.Current.Request.Cookies["demandbasemockip"];
			string mockIp = httpCookie?.Value;
			if (!string.IsNullOrWhiteSpace(mockIp))
			{
				Log.Debug($"[Demandbase Debug][ip] Utilizing mock ip: {mockIp}");
				return mockIp;
			}
			System.Web.HttpContext context = System.Web.HttpContext.Current;
			string ipAddress = context.Request.ServerVariables[DemandbaseContext.LoadBalancerForwardingVariable];
			if (!string.IsNullOrEmpty(ipAddress))
			{
				string[] addresses = ipAddress.Split(',');
				if (addresses.Length != 0)
				{
					Log.Debug($"[Demandbase Debug][ip] Forwarded ip detected.  Using: {addresses[0]}.  Entire IP chain: {string.Join(" => ", addresses)}");
					return addresses[0].Split(':')[0];
				}
			}
			Log.Debug($"[Demandbase Debug][ip] Direct ip detected, using {context.Request.ServerVariables["REMOTE_ADDR"]}");
			return context.Request.ServerVariables["REMOTE_ADDR"];
		}

		public static bool IsLocalSubnet(string ip = null)
		{
			if (string.IsNullOrWhiteSpace(ip))
				ip = GetIpAddress();
			if (SubnetRegex.Match(ip).Success)
				return true;
			return false;
		}
	}
}