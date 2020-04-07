using System;
using System.Collections.Generic;
using SitecoreDemandbase.Data;
using SitecoreDemandbase.Data.Interface;
using SitecoreDemandbase.Pipeline.HttpRequestBegin;

namespace SitecoreDemandbase
{
	public static class DemandbaseContext
	{
		internal static IUserDataService UserServiceSingleton = null;
		internal static string _restApi = null;
		internal static string _key = null;
		internal static string _demandbaseIp = null;
		internal static bool _noHq = false;
		internal static Dictionary<string, DemandbaseAttribute> _attributes = new Dictionary<string, DemandbaseAttribute>();
		internal static Dictionary<string, DemandbaseAttribute> _accountWatch = new Dictionary<string, DemandbaseAttribute>();
		internal static List<Tuple<string,string>> _levels = new List<Tuple<string, string>>();
		internal static string _loadBalancerForwardingVariable;
		public static string Key => _key;
		public static string RestApi => _restApi;
		public static string DemandbaseIp => _demandbaseIp;
		public static string CurrentRequestIp => ValidateUser.GetIpAddress();
		public static string LoadBalancerForwardingVariable => _loadBalancerForwardingVariable;
		public static bool NoHq => _noHq;
		public static IUserDataService User => UserServiceSingleton;
		public static Dictionary<string, DemandbaseAttribute> Attributes => _attributes;
		public static Dictionary<string, DemandbaseAttribute> AccountWatch => _accountWatch;
		public static List<Tuple<string, string>> Levels => _levels;
	}
}
