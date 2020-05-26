namespace SitecoreDemandbase.Data.Interface
{
    public interface IUserDataService
    {
        void ValidateUserData(string ip);
        T GetValue<T>(string key);
        T GetSecondTeirValue<T>(string root, string key);
        dynamic GetFullObject();
    }
}
