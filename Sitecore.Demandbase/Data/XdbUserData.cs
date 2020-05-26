using Sitecore.Analytics;
using Sitecore.Diagnostics;
using SitecoreDemandbase.Data.Interface;
using SitecoreDemandbase.Pipeline.HttpRequestBegin;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Web;

namespace SitecoreDemandbase.Data
{
    public class XdbUserData : IUserDataService
    {
        private readonly HashSet<string> _notFoundIps = new HashSet<string>();
        private int Timeout { get; set; }
        public void ValidateUserData(string ip)
        {
            var contact = Tracker.Current?.Contact;
            if (contact == null)
            {
                Log.Debug($"[Demandbase Debug][xdb] Contact is null, trying to initialize");
                Tracker.Initialize();
                contact = Tracker.Current?.Contact;
            }
            var data = contact.GetFacet<IXdbFacetDemandbaseData>("Demandbase Data");
            if (contact == null || _notFoundIps.Contains(ip) || ValidateUser.IsLocalSubnet(ip))
            {
                Log.Debug($"[Demandbase Debug][xdb] IP is local subnet {ip} skipping data retrieval.");
                if (!string.IsNullOrWhiteSpace(data.DemandBaseData))
                    data.DemandBaseData = "";
                return;
            }
            if (string.IsNullOrWhiteSpace(data.DemandBaseData) || (HasProperty(data.DemandBaseData, "ip") && data.Ip != ip))
            {
                using (HttpClient wc = new HttpClient())
                {
                    wc.Timeout = TimeSpan.FromMilliseconds(Timeout);
                    try
                    {
                        Log.Debug($"[Demandbase Debug][xdb] Retrieving data for ip {ip}");
                        string str = wc.GetStringAsync($"{DemandbaseContext.RestApi}?query={ip}&key={DemandbaseContext.Key}").Result;
                        data.DemandBaseData = str;
                        data.Ip = ip;
                        Log.Debug($"[Demandbase Debug][xdb] Retrieved data for ip {ip} \nJSON:\n\n{str}");

                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("404"))
                        {
                            Log.Warn("[Demandbase Debug][xdb] Ip address was not found in demandbase " + ip, this);
                            _notFoundIps.Add(ip);
                        }
                        else
                        {
                            Log.Warn("[Demandbase Debug][xdb] unable to get demandbase user data for ip " + ip, ex, this);
                        }
                        data.DemandBaseData = "";
                        data.Ip = ip;
                    }
                }
            }
        }
        public static bool HasProperty(dynamic obj, string name)
        {
            Type objType = obj.GetType();
            if (objType == typeof(string))
            {
                ExpandoObject o = JsonNetWrapper.DeserializeObject<ExpandoObject>(obj);
                return ((IDictionary<string, object>)o).ContainsKey(name);
            }
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
            return data != null && data.ContainsKey(key) ? (T)data[key] : default(T);
        }

        public T GetSecondTeirValue<T>(string root, string key)
        {
            if (root == null || key == null) return default(T);
            var data = GetCurrentSessionValue() as IDictionary<string, object>;
            data = data != null && data.ContainsKey(root) ? data[root] as IDictionary<string, object> : null;
            return data != null && data.ContainsKey(key) ? (T)data[key] : default(T);
        }

        public dynamic GetFullObject()
        {
            return GetCurrentSessionValue();
        }
        private ExpandoObject GetCurrentSessionValue(bool abort = false)
        {
            var contact = Tracker.Current?.Contact;
            var tmp = contact?.GetFacet<IXdbFacetDemandbaseData>("Demandbase Data");
            dynamic data = HttpContext.Current.Items["demandbaseObject"];
            if (data == null && tmp?.DemandBaseData != null)
            {
                data = JsonNetWrapper.DeserializeObject<ExpandoObject>(tmp.DemandBaseData);
                HttpContext.Current.Items["demandbaseObject"] = data;
            }
            var dictionary = data as IDictionary<string, object>;
            if (!abort && (dictionary == null || tmp?.DemandBaseData == null || dictionary["ip"]?.ToString() != DemandbaseContext.CurrentRequestIp))
            {
                Log.Debug($"[Demandbase Debug][xdb] Demandbase data not found, going to Demandbase. {DemandbaseContext.CurrentRequestIp}");
                ValidateUserData(ValidateUser.GetIpAddress());
                return GetCurrentSessionValue(true);
            }
            return data as ExpandoObject;
        }
    }
}
