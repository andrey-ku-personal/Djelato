using AutoMapper;
using Djelato.DataAccess.RedisRepositories.Interfaces;
using Djelato.Services.Models;
using Djelato.Services.Notification;
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
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly Mock<INotifier> _mockEmailNotifier;
        private readonly Mock<IRedisRepo> _mockRedis;
        private readonly Mock<IFileService> _mockFile;
        private readonly Mock<IFormFile> _mockFormFile;
        private readonly UserController _controller;

        private readonly UserDTO _correctModel;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockEmailNotifier = new Mock<INotifier>();
            _mockRedis = new Mock<IRedisRepo>();
            _mockFile = new Mock<IFileService>();
            _mockFormFile = new Mock<IFormFile>();
            _mockFormFile.Setup(x => x.Name).Returns("Avatar");

            _controller = new UserController(_mockLogger.Object, _mockMapper.Object, _mockUserService.Object, _mockEmailNotifier.Object, _mockRedis.Object, _mockFile.Object);

            _correctModel = new UserDTO() {Avatar = _mockFormFile.Object, Email = "exist@mail.com" };
        }

        #region Add Async

        [Fact]
        public async Task Should_ReturnStatus400_When_EamilNotUniqueAsync()
        {
            //Arrange
            _mockUserService.Setup(x => x.CheckByEmailAsync(It.IsAny<string>())).ReturnsAsync(true);

            //Action
            var actResult = await _controller.AddAsync(_correctModel);
            var contentResult = actResult as  ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus400_When_ImageNullAsync()
        {
            //Arrange
            var imageRes = new ServiceResult<string>(false, null, null);

            _mockUserService.Setup(x => x.CheckByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockFile.Setup(x => x.SaveAvatarImg(It.IsAny<IFormFile>(), null)).ReturnsAsync(imageRes);            

            //Action
            var actResult = await _controller.AddAsync(_correctModel);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus500_When_UserDidntSaveAsync()
        {
            //Arrange
            var addResult = new ServiceResult(false, null);
            var imageRes = new ServiceResult<string>(true, "some route", null);
            var model = new UserModel();

            _mockUserService.Setup(x => x.AddAsync(It.IsAny<UserModel>())).ReturnsAsync(addResult);
            _mockFile.Setup(x => x.SaveAvatarImg(It.IsAny<IFormFile>(), null)).ReturnsAsync(imageRes);
            _mockMapper.Setup(x => x.Map<UserModel>(_correctModel)).Returns(model);

            //Action
            var actResult = await _controller.AddAsync(_correctModel);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus500_When_KeyDidntCacheAsync()
        {
            //Arrange
            var addResult = new ServiceResult(true, null);
            var imageRes = new ServiceResult<string>(true, "some route", null);
            var model = new UserModel();

            _mockUserService.Setup(x => x.CheckByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockFile.Setup(x => x.SaveAvatarImg(It.IsAny<IFormFile>(), null)).ReturnsAsync(imageRes);
            _mockMapper.Setup(x => x.Map<UserModel>(_correctModel)).Returns(model);
            _mockUserService.Setup(x => x.AddAsync(It.IsAny<UserModel>())).ReturnsAsync(addResult);
            _mockRedis.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny <string>())).ReturnsAsync(false);

            //Action
            var actResult = await _controller.AddAsync(_correctModel);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus200_When_KeyCacheAsync()
        {
            //Arrange
            var addResult = new ServiceResult(true, null);
            var imageRes = new ServiceResult<string>(true, "some route", null);
            var model = new UserModel();

            _mockUserService.Setup(x => x.CheckByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockFile.Setup(x => x.SaveAvatarImg(It.IsAny<IFormFile>(), null)).ReturnsAsync(imageRes);
            _mockMapper.Setup(x => x.Map<UserModel>(_correctModel)).Returns(model);
            _mockUserService.Setup(x => x.AddAsync(It.IsAny<UserModel>())).ReturnsAsync(addResult);
            _mockRedis.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);            

            //Action
            var actResult = await _controller.AddAsync(_correctModel);
            var contentResult = actResult as ContentResult;

            //Assert
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        #endregion

        #region ConfirmEmail

        [Fact]
        public async Task Should_ReturnStatus404_When_KeyNotCorrectAsync()
        {
            //Assert
            string key = "aa1";

            //Act
            var actResult = await _controller.ConfirmEmailAsync(key);
            var contentResult = actResult as ContentResult;

            //Arrange
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus400_When_KeyNotIntoCacheAsync()
        {
            //Assert
            string key = "111111";
            _mockRedis.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync((string)null);

            //Act
            var actResult = await _controller.ConfirmEmailAsync(key);
            var contentResult = actResult as ContentResult;

            //Arrange
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        
        [Fact]
        public async Task Should_ReturnStatus500_When_DidntConfirmeAsync()
        {
            //Assert
            string key = "111111";
            string email = "test@mail.com";
            _mockRedis.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(email);
            _mockUserService.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>())).ReturnsAsync(false);

            //Act
            var actResult = await _controller.ConfirmEmailAsync(key);
            var contentResult = actResult as ContentResult;

            //Arrange
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, contentResult.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnStatus200_When_EmailConfirmeAsync()
        {
            //Assert
            string key = "111111";
            string email = "test@mail.com";
            _mockRedis.Setup(x => x.GetAsync(It.IsAny<string>())).ReturnsAsync(email);
            _mockUserService.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>())).ReturnsAsync(true);

            //Act
            var actResult = await _controller.ConfirmEmailAsync(key);
            var contentResult = actResult as ContentResult;

            //Arrange
            Assert.IsType<ContentResult>(actResult);
            Assert.Equal(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        #endregion

        #region Validation

        [Theory]
        [InlineData("Valid user data")]
        public void Should_ReturnTrue_When_UseValalidAsync(string userModel)
        {
            //Arrange
            var validator = new UserValidator();
            UserDTO dto = GetTestDTOModel(userModel);

            //Act
            var validationResult = validator.Validate(dto);

            //Assert
            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData("Name empty")]
        [InlineData("Avatar object null")]
        [InlineData("Name null")]
        [InlineData("Name incorrect")]
        [InlineData("Email empty")]
        [InlineData("Email null")]
        [InlineData("Email incorrect")]
        [InlineData("Phone null")]
        [InlineData("Phone empty")]
        [InlineData("Phone incorrect")]
        [InlineData("Password empty")]
        [InlineData("Password null")]
        [InlineData("Password incorrect")]
        [InlineData("Passwords isn't confirm")]
        public void Should_ReturnFalse_When_UseInvalalidAsync(string userModel)
        {
            //Arrange
            var validator = new UserValidator();
            UserDTO dto = GetTestDTOModel(userModel);

            //Act
            var validationResult = validator.Validate(dto);

            //Assert
            Assert.False(validationResult.IsValid);
            Assert.True(validationResult.Errors.Count >= 1);
        }


        private UserDTO GetTestDTOModel(string dtoName)
        {
            Dictionary<string, UserDTO> listOfUserModels = new Dictionary<string, UserDTO>
            {
                {
                    "Valid user data",
                    new UserDTO
                    {
                        Avatar = _mockFormFile.Object,
                        Name = "Test",
                        Email = "Test@mail.ru",
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Avatar object null",
                    new UserDTO
                    {
                        Avatar = null,
                        Name = "Test",
                        Email = "Test@mail.ru",
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Name empty",
                    new UserDTO
                    {
                        Name = "",
                        Email = "Test@mail.ru",
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Name null",
                    new UserDTO
                    {
                        Name = null,
                        Email = "Test@mail.ru",
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                 {
                    "Name incorrect",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "@@@Test...",
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Email empty",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "",
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Email null",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = null,
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Email incorrect",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "aasd@asd",
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Phone null",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "aasd@asd",
                        PhoneNumber = null,
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Phone empty",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "aasd@asd",
                        PhoneNumber = "",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                 {
                    "Phone incorrect",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "aasd@asd",
                        PhoneNumber = "aaa1",
                        Password = "1234567A",
                        PasswordConfirm = "1234567A"
                    }
                },

                {
                    "Password empty",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        PhoneNumber = "1234",
                        Password = "",
                        PasswordConfirm = ""
                    }
                },

                {
                    "Password null",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        PhoneNumber = "1234",
                        Password = null,
                        PasswordConfirm = null
                    }
                },

                {
                    "Password incorrect",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        PhoneNumber = "1234",
                        Password = "123",
                        PasswordConfirm = "123"
                    }
                },

                {
                    "Passwords isn't confirm",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        PhoneNumber = "1234",
                        Password = "1234567A",
                        PasswordConfirm = "1234"
                    }
                },

            };

            UserDTO user = listOfUserModels.GetValueOrDefault(dtoName);

            return user;
        }

        #endregion
    }
}
