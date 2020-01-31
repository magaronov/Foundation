namespace Foundation.Cms.Services
{
    public interface ICookieService
    {
        string Get(string cookie);
        void Set(string cookie, string value, bool sessionCookie = false);
        void Remove(string cookie);
    }
}