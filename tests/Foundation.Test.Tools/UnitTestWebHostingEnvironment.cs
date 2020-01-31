using System;
using EPiServer.Web;

namespace Foundation.Test.Tools
{
    public class UnitTestWebHostingEnvironment : IWebHostingEnvironment, IDisposable
    {
        private string _webRootVirtualPath;
        private IWebHostingEnvironment _originalHostingEnvironment;
        private string _webRootPath;

        public UnitTestWebHostingEnvironment()
        {
        }

        public static UnitTestWebHostingEnvironment Auto(string webRootVirtualPath = null)
        {
            var instance = new UnitTestWebHostingEnvironment { WebRootVirtualPath = webRootVirtualPath };
            instance._originalHostingEnvironment = WebHostingEnvironment.Instance;
            WebHostingEnvironment.Instance = instance;
            return instance;
        }

        public void Dispose()
        {
            if (_originalHostingEnvironment != null)
            {
                WebHostingEnvironment.Instance = _originalHostingEnvironment;
                _originalHostingEnvironment = null;
            }
        }

        public virtual string WebRootPath
        {
            get { return _webRootPath ?? Environment.CurrentDirectory; }
            set { _webRootPath = value; }
        }

        public virtual string WebRootVirtualPath
        {
            get { return _webRootVirtualPath ?? "/"; }
            set { _webRootVirtualPath = value; }
        }
    }
}