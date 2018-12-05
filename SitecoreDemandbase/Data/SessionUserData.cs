using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Sitecore.Diagnostics;
using SitecoreDemandbase.Data.Interface;
using SitecoreDemandbase.Pipeline.HttpRequestBegin;

namespace SitecoreDemandbase.Data
{
	public class SessionUserData : IUserDataService
	{
		private readonly HashSet<string> _notFoundIps = new HashSet<string>();
		private int Timeout { get; set; }
		public void ValidateUserData(string ip)
		{
			var session = HttpContext.Current.Session;
			if (session == null || _notFoundIps.Contains(ip))
			{
				Log.Debug($"[Demandbase Debug][session] Session is null or ip not found in demandbase {ip}");
				return;
			}
			string user = session["demandBaseUser"]?.ToString();
			string storedIp = session["demandBaseUserIp"]?.ToString();
			if (string.IsNullOrWhiteSpace(user) || storedIp != ip || ValidateUser.IsLocalSubnet(ip))
			{
				using (HttpClient wc = new HttpClient())
				{
					wc.Timeout = TimeSpan.FromMilliseconds(Timeout);
					try
					{
						Log.Debug($"[Demandbase Debug][session] Retrieving data for ip {ip}");
						string str = wc.GetStringAsync($"{DemandbaseContext.RestApi}?query={ip}&key={DemandbaseContext.Key}").Result;
						session["demandBaseUser"] = str;
						session["demandBaseUserIp"] = ip;
						Log.Debug($"[Demandbase Debug][session] Retrieved data for ip {ip}\nJSON:\n\n{str}");

					}
					catch (Exception ex)
					{
						if (ex.ToString().Contains("404"))
						{
							Log.Warn("[Demandbase Debug][session] Ip address was not found in demandbase " + ip,this);
							_notFoundIps.Add(ip);
						}
						else
						{
							Log.Warn("[Demandbase Debug][session] unable to get demandbase user data for ip " + ip, ex, this);
						}
						
						session["demandBaseUserIp"] = ip;
					}
				}
			}
		}
		public static bool HasProperty(dynamic obj, string name)
		{
			Type objType = obj.GetType();

			if (objType == typeof(ExpandoObject))
			{
				return ((IDictionary<string, object>)obj).ContainsKey(name);
			}

			return objType.GetProperty(name) != null;
		}
		public T GetValue<T>(string key) 
		{
			if (key == null) return default(T);
			var data = GetCurrentSessionValue() as IDictionary<string, object>;
			if (data != null && data.ContainsKey(key))
				return (T)data[key];
			return default(T);
		}

		public T GetSecondTeirValue<T>(string root, string key)
		{
			if (root == null || key == null) return default(T);
			var data = GetCurrentSessionValue() as IDictionary<string, object>;
			if (data != null && data.ContainsKey(root))
			{
				data = data[root] as IDictionary<string, object>;
				if (data != null && data.ContainsKey(key))
					return (T)data[key];
			}
			return default(T);
		}

		public dynamic GetFullObject()
		{
			return GetCurrentSessionValue();
		}

		private ExpandoObject GetCurrentSessionValue(bool abort = false)
		{
			var tmp = HttpContext.Current.Session["demandBaseUser"]?.ToString();
			dynamic data = HttpContext.Current.Items["demandbaseObject"];
			if (data == null && tmp != null)
			{
				HttpContext.Current.Items["demandbaseObject"] = JsonNetWrapper.DeserializeObject<ExpandoObject>(tmp);
				return (ExpandoObject) HttpContext.Current.Items["demandbaseObject"];
			}
			if (!abort && (data == null || ValidateUser.GetIpAddress() != HttpContext.Current.Session["demandBaseUserIp"]?.ToString()))
			{
				Log.Debug($"[Demandbase Debug][xdb] Demandbase data not found, going to Demandbase. {DemandbaseContext.CurrentRequestIp}");
				ValidateUserData(ValidateUser.GetIpAddress());
				return GetCurrentSessionValue(true);
			}
			return data;
		}
	}
}
