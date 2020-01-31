namespace Foundation.Cms.Services
{
    public interface ITrackingCookieService
    {
        void SetTrackingCookie(string value);
        string GetTrackingCookie();
    }
}