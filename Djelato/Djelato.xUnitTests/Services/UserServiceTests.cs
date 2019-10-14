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

        public UserServiceTests()
        {
            _manager = new Mock<IMongoRepoManager>();
            _logger = new Mock<ILogger<UserService>>();
            _mapper = new Mock<IMapper>();
            _hasher = new Mock<IHasher>();
        }

        #region Add user async

        [Fact]
        public async Task Should_ReturnFalse_When_SaltByteIsDefaultAsync()
        {
            //Arrange
            UserService service = new UserService(_manager.Object, _logger.Object, _mapper.Object, _hasher.Object);

            UserModel model = new UserModel()
            {
                Name = "Test",
                Email = "test@email.com",
                Password = default,
                Role = Role.User
            };

            User user = new User();

            _mapper.Setup(x => x.Map<User>(It.IsAny<UserModel>())).Returns(user);
            _hasher.Setup(x => x.GetSalt()).Returns(new byte[16]);

            //Act
            var result = await service.AddAsync(model);

            //Assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnFalse_When_PassIsntHashAsync()
        {
            //Arrange
            UserService service = new UserService(_manager.Object, _logger.Object, _mapper.Object, _hasher.Object);

            UserModel model = new UserModel();

            Random rnd = new Random();
            var salt = new byte[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            User user = new User();

            _mapper.Setup(x => x.Map<User>(model)).Returns(user);
            _hasher.Setup(x => x.GetSalt()).Returns(salt);
            _hasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns((string)null);

            //Act
            var result = await service.AddAsync(model);

            //Assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnFalse_When_UserDidntSaveAsync()
        {
            //Arrange
            UserService service = new UserService(_manager.Object, _logger.Object, _mapper.Object, _hasher.Object);

            UserModel model = new UserModel() { Password = "1234567A" };

            Random rnd = new Random();
            var salt = new byte[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            User user = new User();

            _mapper.Setup(x => x.Map<User>(model)).Returns(user);
            _hasher.Setup(x => x.GetSalt()).Returns(salt);
            _hasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns("some hash");
            _manager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(false));

            //Act
            var result = await service.AddAsync(model);

            _manager.Verify(x => x.UserManager.AddAsync(It.IsAny<User>()));

            //Assert
            Assert.False(result.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_UserSavedAsync()
        {
            //Arrange
            UserService service = new UserService(_manager.Object, _logger.Object, _mapper.Object, _hasher.Object);

            UserModel model = new UserModel() { Password = "1234567A" };

            Random rnd = new Random();
            var salt = new byte[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            User user = new User();

            _mapper.Setup(x => x.Map<User>(model)).Returns(user);
            _hasher.Setup(x => x.GetSalt()).Returns(salt);
            _hasher.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>())).Returns("some hash");
            _manager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(true));

            //Act
            var result = await service.AddAsync(model);

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
            UserService service = new UserService(_manager.Object, _logger.Object, _mapper.Object, _hasher.Object);

            _manager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(false));

            //Act
            var serviceResult = await service.CheckByEmailAsync(It.IsAny<string>());

            //Assert
            Assert.False(serviceResult.IsSuccessful);
        }

        [Fact]
        public async Task Should_ReturnTrue_When_UserExistAsync()
        {
            //Arrange
            UserService service = new UserService(_manager.Object, _logger.Object, _mapper.Object, _hasher.Object);

            _manager.Setup(x => x.UserManager.CheckAsync(It.IsAny<string>())).Returns(Task.FromResult<bool>(true));

            //Act
            var serviceResult = await service.CheckByEmailAsync(It.IsAny<string>());

            //Assert
            Assert.True(serviceResult.IsSuccessful);
        }

        #endregion
    }
}
