using System.Web;

namespace Foundation.Cms.Services
{
    public class TrackingCookieService : ITrackingCookieService
    {
        public const string TrackingCookieName = "_madid";
        private readonly ICookieService _cookieService;

        public TrackingCookieService(ICookieService cookieService)
        {
            _cookieService = cookieService;
        }
       

        public string GetTrackingCookie()
        {
            return _cookieService.Get(TrackingCookieName);
        }

        public void SetTrackingCookie(string value)
        {
            _cookieService.Set(TrackingCookieName, value);
        }
    }
}
