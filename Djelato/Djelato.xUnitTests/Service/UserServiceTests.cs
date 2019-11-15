using AutoMapper;
using Djelato.Common.Entity;
using Djelato.DataAccess.Entity;
using Djelato.DataAccess.Managers.Interfaces;
using Djelato.Services.Models;
using Djelato.Services.PasswordHasher;
using Djelato.Services.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Djelato.xUnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IMongoManager> _mockManager;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHasher> _mockHasher;
        private readonly UserService _service;
        private byte[] _testSalt = new byte[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

    public UserServiceTests()
        {
            _mockManager = new Mock<IMongoManager>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockMapper = new Mock<IMapper>();
            _mockHasher = new Mock<IHasher>();
            _service = new UserService(_mockManager.Object, _mockLogger.Object, _mockMapper.Object, _mockHasher.Object);
        }

        #region Add user async

        [Fact]
        public async Task Should_ReturnFalse_When_SaltByteIsDefaultAsync()
        {
            //Arrange
            UserModel model = new UserModel();

            _mockMapper.Setup(x => x.Map<User>(It.IsAny<UserModel>())).Returns(new User());
            _mockHasher.Setup(x => x.GetSalt()).Returns(new byte[16]);

            //Act
            var result = await _service.AddAsync(model);

            //Assert
            Assert.True(!string.IsNullOrEmpty(result.Message));
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnFalse_When_PassIsntHashAsync()
        {
            //Arrange

            UserModel model = new UserModel();

            _mockMapper.Setup(x => x.Map<User>(model)).Returns(new User());
            _mockHasher.Setup(x => x.GetSalt()).Returns(_testSalt);
            _mockHasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns((string)null);

            //Act
            var result = await _service.AddAsync(model);

            //Assert
            Assert.True(!string.IsNullOrEmpty(result.Message));
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnFalse_When_UserDidntSaveAsync()
        {
            //Arrange
            UserModel model = new UserModel() { Password = "1234567A" };

            _mockMapper.Setup(x => x.Map<User>(model)).Returns(new User());
            _mockHasher.Setup(x => x.GetSalt()).Returns(_testSalt);
            _mockHasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns("some hash");
            _mockManager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).ReturnsAsync(false);

            //Act
            var result = await _service.AddAsync(model);

            _mockManager.Verify(x => x.UserManager.AddAsync(It.IsAny<User>()));

            //Assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_UserSavedAsync()
        {
            //Arrange

            UserModel model = new UserModel() { Password = "1234567A" };

            _mockMapper.Setup(x => x.Map<User>(model)).Returns(new User());
            _mockHasher.Setup(x => x.GetSalt()).Returns(_testSalt);
            _mockHasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns("some hash");
            _mockManager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).ReturnsAsync(true);

            //Act
            var result = await _service.AddAsync(model);

            _mockManager.Verify(x => x.UserManager.AddAsync(It.IsAny<User>()));

            //Assert
            Assert.True(result.IsSuccessful);
        }

        #endregion

        #region Confirm email

        [Fact]
        public async Task Should_ReturnFalse_When_CallNonexistentEmailAsync()
        {
            //Arrange
            string email = "test@mail.com";
            _mockManager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).ReturnsAsync(false);

            //Action
            var result = await _service.ConfirmEmailAsync(email);

            //Assert
            Assert.False(result);        
        }

        [Fact]
        public async Task Should_ReturnFalse_When_DidntUpdateConfirmEmail()
        {
            //Arrange
            string email = "test@mail.com";
            User user = new User() { EmailConfirmed = false };

            var mockReplaceResult = new Mock<ReplaceOneResult>();
            mockReplaceResult.Setup(_ => _.IsAcknowledged).Returns(false);


            _mockManager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockManager.Setup(x => x.UserManager.GetAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockManager.Setup(x => x.UserManager.ReplaceOneAsync(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(mockReplaceResult.Object);

            //Action
            var result = await _service.ConfirmEmailAsync(email);

            //Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_UpdateConfirmEmail()
        {
            //Arrange
            string email = "test@mail.com";
            User user = new User() { EmailConfirmed = false };

            var mockReplaceResult = new Mock<ReplaceOneResult>();
            mockReplaceResult.Setup(_ => _.IsAcknowledged).Returns(true);


            _mockManager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockManager.Setup(x => x.UserManager.GetAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockManager.Setup(x => x.UserManager.ReplaceOneAsync(It.IsAny<string>(), It.IsAny<User>())).ReturnsAsync(mockReplaceResult.Object);

            //Action
            var result = await _service.ConfirmEmailAsync(email);

            //Assert
            Assert.True(result);
        }

        #endregion

        #region CheckByEmail

        [Fact]
        public async Task Should_ReturnFalse_When_UserNonExistentAsync()
        {
            //Arrange
            _mockManager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).ReturnsAsync(false);

            //Act
            var isExist = await _service.CheckByEmailAsync(It.IsAny<string>());

            //Assert
            Assert.False(isExist);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_UserExistAsync()
        {
            //Arrange
            _mockManager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).ReturnsAsync(true);

            //Act
            var isExist = await _service.CheckByEmailAsync(It.IsAny<string>());

            //Assert
            Assert.True(isExist);
        }

        #endregion
    }
}
