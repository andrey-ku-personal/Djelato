using AutoMapper;
using Djelato.Services.Models;
using Djelato.Services.Services.Interfaces;
using Djelato.Web.Controllers;
using Djelato.Web.ViewModel;
using Djelato.Web.ViewModel.FluentApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
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
            var actResult = await _authController.AuthenticateAsync(_authDTO);
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
            var actResult = await _authController.AuthenticateAsync(_authDTO);
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
            var actResult = await _authController.AuthenticateAsync(_authDTO);
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
            var actResult = await _authController.AuthenticateAsync(_authDTO);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        #endregion

        #region Validation

        [Theory]
        [InlineData("Valid auth data")]
        public void Should_ReturnTrue_When_UseValalidAsync(string authModel)
        {
            //Arrange
            var validator = new AuthValidator();
            AuthDTO dto = GetTestDTOModel(authModel);

            //Act
            var validationResult = validator.Validate(dto);

            //Assert
            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData("Null auth email")]
        [InlineData("Empty auth email")]
        [InlineData("Invalid auth email")]
        [InlineData("Null auth pass")]
        [InlineData("Empty auth pass")]
        [InlineData("Without numbers auth pass")]
        [InlineData("Without upper letter auth pass")]
        public void Should_ReturnFalse_When_UseInvalalidAsync(string authModel)
        {
            //Arrange
            var validator = new AuthValidator();
            AuthDTO dto = GetTestDTOModel(authModel);

            //Act
            var validationResult = validator.Validate(dto);

            //Assert
            Assert.False(validationResult.IsValid);
            Assert.True(validationResult.Errors.Count >= 1);
        }


        private AuthDTO GetTestDTOModel(string dtoName)
        {
            Dictionary<string, AuthDTO> listOfUserModels = new Dictionary<string, AuthDTO>
            {
                {
                    "Valid auth data",
                    new AuthDTO
                    {
                        Email = "test@mail.com",
                        Password = "1234567A",
                    }
                },

                {
                    "Null auth email",
                    new AuthDTO
                    {
                        Email = null,
                        Password = "1234567A",
                    }
                },

                {
                    "Empty auth email",
                    new AuthDTO
                    {
                        Email = "",
                        Password = "1234567A",
                    }
                },

                {
                    "Invalid auth email",
                    new AuthDTO
                    {
                        Email = "mail.com",
                        Password = "1234567A",
                    }
                },

                {
                    "Null auth pass",
                    new AuthDTO
                    {
                        Email = "test@mail.com",
                        Password = null,
                    }
                },

                {
                    "Empty auth pass",
                    new AuthDTO
                    {
                        Email = "test@mail.com",
                        Password = "",
                    }
                },

                {
                    "Without numbers auth pass",
                    new AuthDTO
                    {
                        Email = "test@mail.com",
                        Password = "aaaaaaAAAAA",
                    }
                },


                {
                    "Without upper letter auth pass",
                    new AuthDTO
                    {
                        Email = "test@mail.com",
                        Password = "1111aaaaa",
                    }
                },

            };

            AuthDTO user = listOfUserModels.GetValueOrDefault(dtoName);

            return user;
        }

        #endregion
    }
}
