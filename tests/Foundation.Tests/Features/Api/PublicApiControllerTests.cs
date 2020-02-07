using EPiServer;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using FluentAssertions;
using Foundation.Cms.Identity;
using Foundation.Cms.Personalization;
using Foundation.Cms.Services;
using Foundation.Commerce.Customer;
using Foundation.Commerce.Customer.Services;
using Foundation.Commerce.Customer.ViewModels;
using Foundation.Features.Api;
using Foundation.Infrastructure.Services;
using Foundation.Test.Tools.Fakes;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Moq;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace Foundation.Tests.Features.Api
{
    public class PublicApiControllerTests
    {
        [Fact]
        public void Signout_WhenRedirectingToAction()
        {
            var result = _subject.SignOut();

            _customerService.Verify(_ => _.SignOut(), Times.Once);
            _trackingCookieService.Verify(_ => _.SetTrackingCookie(It.IsAny<string>()), Times.Once);
            
            result.Should().BeOfType<System.Web.Mvc.RedirectToRouteResult>().Which.RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public async Task Login_WhenUserDoesNotExist()
        {
            _customerService.Setup(_ => _.UserManager)
                .Returns(() => Setup_ServiceAccessors(Setup_UserManager(null)));

            var result = await _subject.Login("mark", It.IsAny<string>());
            
            _customerService.Verify(s => s.SignOut(), Times.Once);
            result.Should().BeOfType<EmptyResult>();
        }

        [Fact]
        public async Task Login_WhenUserDoesExist()
        {
            var userManager = Setup_UserManager((store) =>
            {
                var userPasswordStore = store.As<IUserEmailStore<SiteUser>>();
                userPasswordStore.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new SiteUser
                {
                    Username = "mark"
                }));

            });
            _customerService.Setup(_ => _.UserManager)
                .Returns(() => Setup_ServiceAccessors(userManager));

            _customerService.Setup(_ => _.SignInManager)
                .Returns(() => Setup_ServiceAccessors(Setup_SignInManager(userManager)));

            var result = await _subject.Login("mark", "http://foundation");

            
            result.Should().BeOfType<RedirectResult>().Which.Url.Should().Be("http://foundation");
        }

        [Fact]
        public async Task RegisterAccount_WhenModelStateisInvalid()
        {
            var viewModel = new RegisterAccountViewModel
            {
                Address = new AddressModel()
            };

            _subject.ModelState.AddModelError("test", "not valid");

            var result = await _subject.RegisterAccount(viewModel);

            _customerService.Verify(s => s.CreateUser(It.IsAny<SiteUser>()), Times.Never);
            result.Should().BeEquivalentTo(new JsonResult()
            {
                Data = new
                {
                    success = false,
                    errors = new string[] { "not valid" }
                }
            });
        }

        [Fact]
        public async Task RegisterAccount_WhenSuccess()
        {
            var identityResult = new IdentityResult();
            typeof(IdentityResult).GetProperty("Succeeded").SetValue(identityResult, true, null);

            _customerService.Setup(x => x.CreateUser(It.IsAny<SiteUser>()))
                .Returns(Task.FromResult(new IdentityContactResult
                {
                    IdentityResult = identityResult,
                    FoundationContact = FoundationContact.New()
                }));

            var viewModel = new RegisterAccountViewModel
            {
                Address = new AddressModel()
            };

            var result = await _subject.RegisterAccount(viewModel);

            _customerService.Verify(s => s.CreateUser(It.IsAny<SiteUser>()), Times.Once);
            _addressBookService.Verify(x => x.Save(It.IsAny<AddressModel>(), It.IsAny<FoundationContact>()), Times.Once);
            result.Should().BeOfType<EmptyResult>();
        }

        [Fact]
        public async Task RegisterAccount_WhenFailure()
        {
            _customerService.Setup(x => x.CreateUser(It.IsAny<SiteUser>()))
                .Returns(Task.FromResult(new IdentityContactResult
                {
                    IdentityResult = new IdentityResult("error"),
                    FoundationContact = FoundationContact.New()
                }));

            var viewModel = new RegisterAccountViewModel
            {
                Address = new AddressModel()
            };

            var result = await _subject.RegisterAccount(viewModel);

            _customerService.Verify(s => s.CreateUser(It.IsAny<SiteUser>()), Times.Once);
            _addressBookService.Verify(x => x.Save(It.IsAny<AddressModel>(), It.IsAny<FoundationContact>()), Times.Never);
            result.Should().BeEquivalentTo(new JsonResult()
            {
                Data = new
                {
                    success = false,
                    errors = new string[] { "error" }
                }
            });
        }

        [Fact]
        public async Task InternalLogin_WhenModelStateisInvalid()
        {
            _httpRequestBaseMock.Setup(x => x.UrlReferrer).Returns(new Uri("http://mark"));
            _subject.ModelState.AddModelError("test", "not valid");

            var result = await _subject.InternalLogin(new LoginViewModel());

            result.Should().BeEquivalentTo(new JsonResult()
            {
                Data = new
                {
                    success = false,
                    errors = new string[] { "not valid" }
                }
            });
        }

        [Fact]
        public async Task InternalLogin_WhenSuccessAndRememberMeTrue()
        {
            var siteUser = new SiteUser
            {
                Username = "test@email.com",
                Email = "test@email.com"
            };

            _httpRequestBaseMock.Setup(x => x.UrlReferrer).Returns(new Uri("http://mark"));
            _customerService.Setup(x => x.GetSiteUser(It.IsAny<string>())).Returns(siteUser);
            var userManager = Setup_UserManager((store) =>
            {
                var userPasswordStore = store.As<IUserPasswordStore<SiteUser>>();
                userPasswordStore.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(siteUser));

            });

            _customerService.Setup(_ => _.UserManager)
                .Returns(() => Setup_ServiceAccessors(userManager));

            _customerService.Setup(_ => _.SignInManager)
                .Returns(() => Setup_ServiceAccessors(Setup_SignInManager(userManager)));
            var result = await _subject.InternalLogin(new LoginViewModel
            {
                Password = "password",
                RememberMe = true,
            });

            result.Should().BeEquivalentTo(new JsonResult()
            {
                Data = new
                {
                    success = false,
                    errors = new string[] { "not valid" }
                }
            });
        }

        public PublicApiControllerTests()
        {

            _addressBookService = new Mock<IAddressBookService>();
            _campaignService = new Mock<ICampaignService>();
            _cmsTrackingService = new Mock<ICmsTrackingService>();
            _contentLoader = new Mock<IContentLoader>();
            _customerService = new Mock<ICustomerService>();
            _httpContextBase = new Mock<HttpContextBase>();
            _localizationService = new Mock<FakeLocalizationService>();
            _urlResolver = new Mock<IUrlResolver>();
            _trackingCookieService = new Mock<ITrackingCookieService>();
            _httpRequestBaseMock = new Mock<HttpRequestBase>();
            _httpContextBase.SetupGet(x => x.Request).Returns(_httpRequestBaseMock.Object);

            _subject = new PublicApiController(_localizationService.Object,
                _contentLoader.Object,
                _addressBookService.Object,
                _customerService.Object,
                _campaignService.Object,
                _urlResolver.Object,
                _cmsTrackingService.Object,
                _httpContextBase.Object,
                _trackingCookieService.Object);
            _subject.ControllerContext = new ControllerContext(_httpContextBase.Object, new RouteData(), _subject);
        }

        private PublicApiController _subject;
        private Mock<IContentLoader> _contentLoader;
        private Mock<IAddressBookService> _addressBookService;
        private Mock<FakeLocalizationService> _localizationService;
        private Mock<ICustomerService> _customerService;
        private Mock<ICampaignService> _campaignService;
        private Mock<IUrlResolver> _urlResolver;
        private Mock<ICmsTrackingService> _cmsTrackingService;
        private Mock<HttpContextBase> _httpContextBase;
        private Mock<ITrackingCookieService> _trackingCookieService;
        private Mock<HttpRequestBase> _httpRequestBaseMock;

        private ServiceAccessor<T> Setup_ServiceAccessors<T>(T value)
        {
            return new ServiceAccessor<T>(() => value);
        }

        private ApplicationUserManager<SiteUser> Setup_UserManager(Action<Mock<IUserStore<SiteUser>>> configureStore = null)
        {
            var userStore = new Mock<IUserStore<SiteUser>>();
            configureStore?.Invoke(userStore);
            return new ApplicationUserManager<SiteUser>(userStore.Object);
        }

        private ApplicationSignInManager<SiteUser> Setup_SignInManager(ApplicationUserManager<SiteUser> userManager)
        {
            var authenticationManager = new Mock<IAuthenticationManager>();
            return new ApplicationSignInManager<SiteUser>(userManager, authenticationManager.Object, new ApplicationOptions());
        }

    }
}
