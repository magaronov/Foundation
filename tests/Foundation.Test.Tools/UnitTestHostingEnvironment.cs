
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using EPiServer.Web;
using EPiServer.Web.Hosting;

namespace Foundation.Test.Tools
{
    public class UnitTestHostingEnvironment : IHostingEnvironment, IDisposable
    {
        private readonly List<VirtualPathProvider> _virtualPathProviders = new List<VirtualPathProvider>();
        private string _applicationVirtualPath;
        private string _applicationID;
        private IHostingEnvironment _originalHostingEnvironment;
        private string _applicationPhysicalPath;

        public UnitTestHostingEnvironment()
        {
            _virtualPathProviders.Add(new FallbackVirtualPathProvider(this));
        }

#region IHostingEnvironment Members

        public virtual void RegisterVirtualPathProvider(VirtualPathProvider virtualPathProvider)
        {
            InitializeVPP(virtualPathProvider);
            _virtualPathProviders.Add(virtualPathProvider);
        }

        public IEnumerable<VirtualPathProvider> VirtualPathProviders
        {
            get { return _virtualPathProviders; }
        }

        public virtual VirtualPathProvider VirtualPathProvider
        {
            get { return _virtualPathProviders[_virtualPathProviders.Count -1]; }
        }

        public virtual string MapPath(string virtualPath)
        {
            return Path.Combine(ApplicationPhysicalPath, VirtualPathUtility.ToAbsolute(virtualPath, ApplicationVirtualPath).Replace('/', '\\').TrimStart('\\'));
        }

        public virtual string ApplicationID
        {
            get { return _applicationID ?? String.Empty; }
            set { _applicationID = value; }
        }

        public virtual string ApplicationPhysicalPath
        {
            get { return _applicationPhysicalPath ?? Environment.CurrentDirectory; }
            set { _applicationPhysicalPath = value; }
        }

        public virtual string ApplicationVirtualPath
        {
            get { return _applicationVirtualPath ?? "/"; }
            set { _applicationVirtualPath = value; }
        }

#endregion

        private void InitializeVPP(VirtualPathProvider virtualPathProvider)
        {
            //Calling initialize will set Previous property
            MethodInfo initializeMethod = typeof(VirtualPathProvider).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, new Type[] { typeof(VirtualPathProvider) }, null);
            initializeMethod.Invoke(virtualPathProvider, new object[] { VirtualPathProvider }); ;
        }

        /// <summary>
        /// This is a stripped-down rip-off of System.Web.Hosting.MapPathBasedVirtualPathProvider which is used as a fallback provider.
        /// </summary>
        private class FallbackVirtualPathProvider : VirtualPathProvider
        {
            private IHostingEnvironment _hostingEnvironment;

            public IHostingEnvironment HostingEnv
            {
                get { return _hostingEnvironment; }
                set { _hostingEnvironment = value; }
            }

            public FallbackVirtualPathProvider(IHostingEnvironment hostingEnvironment)
            {
                this.HostingEnv = hostingEnvironment;
            }

            // Methods
            public override bool DirectoryExists(string virtualDir)
            {
                return Directory.Exists(HostingEnv.MapPath(virtualDir));
            }

            public override bool FileExists(string virtualPath)
            {
                return File.Exists(HostingEnv.MapPath(virtualPath));
            }

            public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
            {
                if (virtualPathDependencies == null)
                {
                    return null;
                }
                StringCollection strings = null;
                foreach (string str in virtualPathDependencies)
                {
                    string str2 = HostingEnv.MapPath(str);
                    if (strings == null)
                    {
                        strings = new StringCollection();
                    }
                    strings.Add(str2);
                }
                if (strings == null)
                {
                    return null;
                }
                string[] array = new string[strings.Count];
                strings.CopyTo(array, 0);
                return new CacheDependency(array, null, utcStart);
            }

            public override VirtualDirectory GetDirectory(string virtualDir)
            {
                return new FallbackVirtualDirectory(virtualDir);
            }

            public override VirtualFile GetFile(string virtualPath)
            {
                return new FallbackVirtualFile(virtualPath);
            }
        }

        private class FallbackVirtualFile : VirtualFile
        {
            public FallbackVirtualFile(string virtualPath) 
                : base(virtualPath) { }

            public override Stream Open()
            {
                throw new NotImplementedException();
            }
        }

        private class FallbackVirtualDirectory : VirtualDirectory
        {
            public FallbackVirtualDirectory(string virtualDir)
                : base(virtualDir) { }

            public override IEnumerable Directories
            {
                get { return Enumerable.Empty<VirtualDirectory>(); }
            }

            public override IEnumerable Files
            {
                get { return Enumerable.Empty<VirtualFile>(); }
            }

            public override IEnumerable Children
            {
                get { return Enumerable.Empty<VirtualFileBase>(); }
            }

        }

        public static UnitTestHostingEnvironment Auto(string applicationVirtualPath = null)
        {
            var instance = new UnitTestHostingEnvironment { ApplicationVirtualPath = applicationVirtualPath };
            instance._originalHostingEnvironment = GenericHostingEnvironment.Instance;
            GenericHostingEnvironment.Instance = instance;
            WebHostingEnvironment.Instance = new UnitTestWebHostingEnvironment { WebRootVirtualPath = applicationVirtualPath };
            return instance;
        }

        public void Dispose()
        {
            if (_originalHostingEnvironment != null)
            {
                GenericHostingEnvironment.Instance = _originalHostingEnvironment;
                _originalHostingEnvironment = null;
                WebHostingEnvironment.Instance = null;
            }
        }
    }
}
