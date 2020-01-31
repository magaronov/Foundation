using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.Web.Routing;

namespace Moq.EPiServer
{
    public static class ContentMockExtensions
    {
        public static Mock<IContent> Setup(this Mock<IContent> contentMock, ContentReference contentLink = null, string name = null, ContentReference parentLink = null, Guid? contentGuid = null, int contentTypeId = 0, bool isDeleted = false)
        {
            contentMock.SetupProperty(x => x.Name, name);
            contentMock.SetupProperty(x => x.ContentLink, contentLink);
            contentMock.SetupProperty(x => x.ContentGuid, contentGuid ?? Guid.Empty);
            contentMock.SetupProperty(x => x.ContentTypeID, contentTypeId);
            contentMock.SetupProperty(x => x.ParentLink, parentLink);
            contentMock.SetupProperty(x => x.IsDeleted, isDeleted);

            return contentMock;
        }

        public static Mock<IContent> SetupVersionable(this Mock<IContent> contentMock, VersionStatus status = VersionStatus.Published, DateTime? startPublish = null, DateTime? stopPublish = null, bool isPendingPublish = false)
        {
            var versionable = contentMock.As<IVersionable>();
            versionable.SetupProperty(x => x.Status, status);
            versionable.SetupProperty(x => x.IsPendingPublish, isPendingPublish);
            versionable.SetupProperty(x => x.StartPublish, startPublish);
            versionable.SetupProperty(x => x.StopPublish, stopPublish);

            return contentMock;
        }

        public static Mock<IContent> SetupLocalizable(this Mock<IContent> contentMock, CultureInfo language, CultureInfo masterLanguage = null, IEnumerable<CultureInfo> existingLanguages = null)
        {
            var localizable = contentMock.As<ILocalizable>();
            localizable.SetupProperty(x => x.Language, language);
            localizable.SetupProperty(x => x.MasterLanguage, masterLanguage ?? language);

            if (existingLanguages == null)
            {
                existingLanguages = new List<CultureInfo> { language };
                if (masterLanguage != null) existingLanguages = existingLanguages.Concat(new[] { masterLanguage });
            }

            localizable.SetupProperty(x => x.ExistingLanguages, existingLanguages);

            return contentMock;
        }

        public static Mock<IContent> SetupResourceable(this Mock<IContent> contentMock, Guid contentAssetsID)
        {
            var resourceable = contentMock.As<IResourceable>();
            resourceable.SetupProperty(x => x.ContentAssetsID, contentAssetsID);

            return contentMock;
        }

        public static Mock<IContent> SetupReadOnly(this Mock<IContent> contentMock, bool isReadOnly = false)
        {
            var readOnly = contentMock.As<IReadOnly>();
            readOnly.SetupGet(x => x.IsReadOnly).Returns(isReadOnly);

            return contentMock;
        }

        public static Mock<IContent> SetupModifiedTrackable(this Mock<IContent> contentMock, bool isModified = false)
        {
            var modifiedTrackable = contentMock.As<IModifiedTrackable>();
            modifiedTrackable.SetupGet(x => x.IsModified).Returns(isModified);

            return contentMock;
        }

        public static Mock<IContent> SetupRoutable(this Mock<IContent> contentMock, string routeSegment = null)
        {
            var routable = contentMock.As<IRoutable>();
            routable.SetupProperty(x => x.RouteSegment, routeSegment);

            return contentMock;
        }
    }
}
