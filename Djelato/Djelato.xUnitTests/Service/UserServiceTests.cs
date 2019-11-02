using AutoMapper;
using Djelato.Common.Entity;
using Djelato.DataAccess.Entity;
using Djelato.DataAccess.Managers.Interfaces;
using Djelato.Services.Models;
using Djelato.Services.PasswordHasher;
using Djelato.Services.Services;
using Microsoft.Extensions.Logging;
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
        private readonly Mock<IMongoRepoManager> _manager;
        private readonly Mock<ILogger<UserService>> _logger;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IHasher> _hasher;
        private readonly UserService _service;
        private byte[] _testSalt = new byte[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

    public UserServiceTests()
        {
            _manager = new Mock<IMongoRepoManager>();
            _logger = new Mock<ILogger<UserService>>();
            _mapper = new Mock<IMapper>();
            _hasher = new Mock<IHasher>();
            _service = new UserService(_manager.Object, _logger.Object, _mapper.Object, _hasher.Object);
        }

        #region Add user async

        [Fact]
        public async Task Should_ReturnFalse_When_SaltByteIsDefaultAsync()
        {
            //Arrange
            UserModel model = new UserModel();

            _mapper.Setup(x => x.Map<User>(It.IsAny<UserModel>())).Returns(new User());
            _hasher.Setup(x => x.GetSalt()).Returns(new byte[16]);

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

            _mapper.Setup(x => x.Map<User>(model)).Returns(new User());
            _hasher.Setup(x => x.GetSalt()).Returns(_testSalt);
            _hasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns((string)null);

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

            _mapper.Setup(x => x.Map<User>(model)).Returns(new User());
            _hasher.Setup(x => x.GetSalt()).Returns(_testSalt);
            _hasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns("some hash");
            _manager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(false));

            //Act
            var result = await _service.AddAsync(model);

            _manager.Verify(x => x.UserManager.AddAsync(It.IsAny<User>()));

            //Assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_UserSavedAsync()
        {
            //Arrange

            UserModel model = new UserModel() { Password = "1234567A" };

            _mapper.Setup(x => x.Map<User>(model)).Returns(new User());
            _hasher.Setup(x => x.GetSalt()).Returns(_testSalt);
            _hasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns("some hash");
            _manager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(true));

            //Act
            var result = await _service.AddAsync(model);

            _manager.Verify(x => x.UserManager.AddAsync(It.IsAny<User>()));

            //Assert
            Assert.True(result.IsSuccessful);
        }

        #endregion

        #region CheckByEmail

        [Fact]
        public async Task Should_ReturnFalse_When_UserNonExistentAsync()
        {
            //Arrange
            _manager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(false));

            //Act
            var isExist = await _service.CheckByEmailAsync(It.IsAny<string>());

            //Assert
            Assert.False(isExist);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_UserExistAsync()
        {
            //Arrange
            _manager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(true));

            //Act
            var isExist = await _service.CheckByEmailAsync(It.IsAny<string>());

            //Assert
            Assert.True(isExist);
        }

        #endregion
    }
}
