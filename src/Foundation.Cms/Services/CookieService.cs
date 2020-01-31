using EPiServer.ServiceLocation;
using System;
using System.Web;

namespace Foundation.Cms.Services
{
    public class CookieService : ICookieService
    {
        private readonly ServiceAccessor<HttpContextBase> _httpContextAccessor;

        public CookieService(ServiceAccessor<HttpContextBase> httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual string Get(string cookie)
        {
            if (_httpContextAccessor() == null)
            {
                return null;
            }

            return _httpContextAccessor().Request.Cookies[cookie]?.Value;
        }

        public virtual void Set(string cookie, string value, bool sessionCookie = false)
        {
            if (_httpContextAccessor() == null)
            {
                return;
            }

            var httpCookie = new HttpCookie(cookie)
            {
                Value = value,
                HttpOnly = true,
                Secure = _httpContextAccessor().Request?.IsSecureConnection ?? false
            };


            if (!sessionCookie)
            {
                httpCookie.Expires = DateTime.Now.AddYears(1);
            }

            Set(_httpContextAccessor().Response.Cookies, httpCookie);
            _httpContextAccessor().Request.Cookies.Set(httpCookie);
        }

        public virtual void Remove(string cookie)
        {
            if (_httpContextAccessor() == null)
            {
                return;
            }

            var httpCookie = new HttpCookie(cookie)
            {
                Expires = DateTime.Now.AddDays(-1),
#if RELEASE
                HttpOnly = true,
                Secure = true
#endif
            };

            Set(_httpContextAccessor().Response.Cookies, httpCookie);
            _httpContextAccessor().Request.Cookies.Set(httpCookie);
        }

        private void Set(HttpCookieCollection cookieCollection, HttpCookie cookie) => cookieCollection.Add(cookie);
    }
}