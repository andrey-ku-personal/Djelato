using AutoMapper;
using Djelato.Services.Models;
using Djelato.Services.Services.Interfaces;
using Djelato.Web.Controllers;
using Djelato.Web.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Djelato.xUnitTests.Web
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AuthController>> _mockLogger;

        private readonly AuthDTO _authDTO;

        private readonly AuthController _authController;
        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AuthController>>();

            _authDTO = new AuthDTO() { Email = "test@mail.com" };

            _authController = new AuthController(_mockAuthService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        #region Authenticate

        [Fact]
        public async Task Should_ReturnStatus400_When_EmailDontExist()
        {
            //Arrange
            _mockAuthService.Setup(x => x.CheckUserAsync(It.IsAny<string>())).ReturnsAsync(false);

            //Act
            var actResult = await _authController.AuthenticationAsync(_authDTO);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus400_When_DontAuthorizeExist()
        {
            //Arrange
            var model = new AuthModel();
            var authResult = new ServiceResult(false, null);

            _mockAuthService.Setup(x => x.CheckUserAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockMapper.Setup(x => x.Map<AuthModel>(model));
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<AuthModel>())).ReturnsAsync(authResult);

            //Act
            var actResult = await _authController.AuthenticationAsync(_authDTO);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus500_When_DontGenerateTokenExist()
        {
            //Arrange
            var model = new AuthModel();
            var authResult = new ServiceResult(true, null);
            var tokenResult = new ServiceResult<string>(false, null, null);

            _mockAuthService.Setup(x => x.CheckUserAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockMapper.Setup(x => x.Map<AuthModel>(_authDTO)).Returns(model);
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<AuthModel>())).ReturnsAsync(authResult);
            _mockAuthService.Setup(x => x.GetTokenAsync(It.IsAny<string>())).ReturnsAsync(tokenResult);

            //Act
            var actResult = await _authController.AuthenticationAsync(_authDTO);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus200_When_TokenGeneratedExist()
        {
            //Arrange
            var model = new AuthModel();
            var authResult = new ServiceResult(true, null);
            var tokenResult = new ServiceResult<string>(true, "Some token", null);

            _mockAuthService.Setup(x => x.CheckUserAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockMapper.Setup(x => x.Map<AuthModel>(_authDTO)).Returns(model);
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<AuthModel>())).ReturnsAsync(authResult);
            _mockAuthService.Setup(x => x.GetTokenAsync(It.IsAny<string>())).ReturnsAsync(tokenResult);

            //Act
            var actResult = await _authController.AuthenticationAsync(_authDTO);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        #endregion
    }
}
