using EPiServer.Framework.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Foundation.Test.Tools.Fakes
{
    public class FakeLocalizationService : LocalizationService
    {
        public FakeLocalizationService() : base(new Mock<ResourceKeyHandler>().Object)
        {

        }

        public override IEnumerable<CultureInfo> AvailableLocalizations => throw new NotImplementedException();

        protected override IEnumerable<ResourceItem> GetAllStringsByCulture(string originalKey, string[] normalizedKey, CultureInfo culture) => throw new NotImplementedException();
        protected override string LoadString(string[] normalizedKey, string originalKey, CultureInfo culture) => throw new NotImplementedException();
    }
}
