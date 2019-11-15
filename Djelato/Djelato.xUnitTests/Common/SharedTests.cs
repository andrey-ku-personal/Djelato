using Djelato.Common.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Djelato.xUnitTests.Common
{
    public class SharedTests
    {
        [Fact]
        public void Should_GenerateRandomNumbers()
        {
            //Arrange

            //Act
            int value1 = RandomGenerator.RandomNumber(1, 1000000);
            int value2 = RandomGenerator.RandomNumber(1, 1000000);
            int value3 = RandomGenerator.RandomNumber(1, 1000000);

            //Assert
            Assert.True(value1 != value2 && value1 != value2);
            Assert.True(value2 != value1 && value2 != value3);
            Assert.True(value3 != value1 && value3 != value2);
        }
    }
}
