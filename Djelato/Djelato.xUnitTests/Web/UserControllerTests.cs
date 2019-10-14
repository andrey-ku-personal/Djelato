using AutoMapper;
using Djelato.Common.Entity;
using Djelato.Services.Models;
using Djelato.Services.Services;
using Djelato.Services.Services.Interfaces;
using Djelato.Web.Controllers;
using Djelato.Web.ViewModel;
using Djelato.Web.ViewModel.FluentApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Djelato.xUnitTests.Web
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserController>> _mockLogger;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserController>>();
        }

        #region Add Async

        [Fact]
        public async Task Should_ReturnBadRequest_When_EamilNotUniqueAsync()
        {
            //Arrange
            UserController controller = new UserController(_mockLogger.Object, _mockMapper.Object, _mockUserService.Object);
            UserDTO dto = new UserDTO()
            {
                Email = "exist@mail.com"
            };

            var serviceResult = new ServiceResult(true, null);

            _mockUserService.Setup(x => x.CheckByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(serviceResult));

            //Action
            var actResult = await controller.AddAsync(dto);
            var badRequestResult = actResult as BadRequestObjectResult;

            //Assert
            Assert.IsType<BadRequestObjectResult>(actResult);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }
                
        [Fact]
        public async Task Should_ReturnInternalServerError_When_UserdidntSaveAsync()
        {
            //Arrange
            UserController controller = new UserController(_mockLogger.Object, _mockMapper.Object, _mockUserService.Object);
            UserDTO dto = new UserDTO()
            {
                Email = "exist@mail.com"
            };

            var serviceResult = new ServiceResult(false, null);

            _mockUserService.Setup(x => x.CheckByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(serviceResult));
            _mockUserService.Setup(x => x.AddAsync(It.IsAny<UserModel>())).Returns(Task.FromResult(serviceResult));

            //Action
            var actResult = await controller.AddAsync(dto);
            var internalServerErrorRequest = actResult as ObjectResult;

            //Assert
            Assert.IsType<ObjectResult>(actResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorRequest.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnOk_When_UserSavedAsync()
        {
            //Arrange
            UserController controller = new UserController(_mockLogger.Object, _mockMapper.Object, _mockUserService.Object);
            UserDTO dto = new UserDTO()
            {
                Email = "exist@mail.com"
            };

            var checkResult = new ServiceResult(false, null);
            var addResult = new ServiceResult(true, null);

            _mockUserService.Setup(x => x.CheckByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(checkResult));
            _mockUserService.Setup(x => x.AddAsync(It.IsAny<UserModel>())).Returns(Task.FromResult(addResult));

            //Action
            var actResult = await controller.AddAsync(dto);
            var badRequestResult = actResult as OkObjectResult;

            //Assert
            Assert.IsType<OkObjectResult>(actResult);
            Assert.Equal(StatusCodes.Status200OK, badRequestResult.StatusCode);
        }

        [Theory]
        [InlineData("Valid user data")]
        public void Should_ReturnTrue_When_UseValalidAsync(string userModel)
        {
            //Arrange
            var validator = new UserValidator();
            UserDTO userDTO = GetTestDTOModel(userModel);

            //Act
            var validationResult = validator.Validate(userDTO);

            //Assert
            Assert.True(validationResult.IsValid);
        }

        [Theory]
        [InlineData("Name empty")]
        [InlineData("Name null")]
        [InlineData("Email empty")]
        [InlineData("Email null")]
        [InlineData("Email incorrect")]
        [InlineData("Password empty")]
        [InlineData("Password null")]
        [InlineData("Password incorrect")]
        [InlineData("Passwords isn't confirm")]
        public void Should_ReturnFalse_When_UseInvalalidAsync(string userModel)
        {
            //Arrange
            var validator = new UserValidator();
            UserDTO userDTO = GetTestDTOModel(userModel);

            //Act
            var validationResult = validator.Validate(userDTO);

            //Assert
            Assert.False(validationResult.IsValid);
        }

        #endregion

        private UserDTO GetTestDTOModel(string dtoName)
        {
            Dictionary<string, UserDTO> listOfUserModels = new Dictionary<string, UserDTO>
            {
                {
                    "Valid user data",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        Password = "1234567A",
                        ConfirmPassword = "1234567A"
                    }
                },

                {
                    "Name empty",
                    new UserDTO
                    {
                        Name = "",
                        Email = "Test@mail.ru",
                        Password = "1234567A",
                        ConfirmPassword = "1234567A"
                    }
                },

                {
                    "Name null",
                    new UserDTO
                    {
                        Name = null,
                        Email = "Test@mail.ru",
                        Password = "1234567A",
                        ConfirmPassword = "1234567A"
                    }
                },

                {
                    "Email empty",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "",
                        Password = "1234567A",
                        ConfirmPassword = "1234567A"
                    }
                },

                {
                    "Email null",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = null,
                        Password = "1234567A",
                        ConfirmPassword = "1234567A"
                    }
                },

                {
                    "Email incorrect",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "aasd@asd",
                        Password = "1234567A",
                        ConfirmPassword = "1234567A"
                    }
                },

                {
                    "Password empty",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        Password = "",
                        ConfirmPassword = ""
                    }
                },

                {
                    "Password null",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        Password = null,
                        ConfirmPassword = null
                    }
                },

                {
                    "Password incorrect",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        Password = "123",
                        ConfirmPassword = "123"
                    }
                },

                {
                    "Passwords isn't confirm",
                    new UserDTO
                    {
                        Name = "Test",
                        Email = "Test@mail.ru",
                        Password = "1234567A",
                        ConfirmPassword = "1234"
                    }
                },

            };

            UserDTO user = listOfUserModels.GetValueOrDefault(dtoName);

            return user;
        }
                
    }
}
