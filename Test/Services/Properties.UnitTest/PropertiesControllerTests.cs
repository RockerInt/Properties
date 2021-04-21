using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Properties.Controllers;
using Properties.Models;
using Properties.Models.Parameters;
using Properties.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Properties.UnitTest
{
    public class PropertiesControllerTests
    {
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<ILogger<PropertiesController>> _loggerMock;
        private PropertiesController PropertyController;
        private Guid FakePropertyId;
        private Property FakeProperty;

        public PropertiesControllerTests()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _loggerMock = new Mock<ILogger<PropertiesController>>();
        }

        [SetUp]
        public void Setup()
        {
            FakePropertyId = Guid.Parse("8cc32b40-578d-47c1-bb9f-63240737243f");
            FakeProperty = GetPropertyFake(FakePropertyId);

            _propertyRepositoryMock.Setup(x => x.GetProperty(It.IsAny<Guid>()))
                .Returns(Task.FromResult(FakeProperty));

            _propertyRepositoryMock.Setup(x => x.GetProperties(It.IsAny<PropertiesParameters>()))
                .Returns(Task.FromResult(new List<Property>(){ FakeProperty }));

            _propertyRepositoryMock.Setup(x => x.CreateProperty(It.IsAny<Property>()))
                .Returns(Task.FromResult(FakeProperty));

            _propertyRepositoryMock.Setup(x => x.UpdateProperty(It.IsAny<Property>()))
                .Returns(Task.FromResult(FakeProperty));

            _propertyRepositoryMock.Setup(x => x.DeleteProperty(It.IsAny<Property>()))
                .Returns(Task.FromResult(true));


            PropertyController = new PropertiesController(
                _loggerMock.Object,
                _propertyRepositoryMock.Object
            );
        }

        [Test]
        public void GetPropertyTest()
        {
            var actionResult = PropertyController.GetProperty(FakePropertyId).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Property>().IdProperty, FakeProperty.IdProperty);
        }

        [Test]
        public void GetPropertiesTest()
        {
            var actionResult = PropertyController.GetProperties(new PropertiesParameters()).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntityListSimple<Property>().FirstOrDefault().IdProperty, FakeProperty.IdProperty);
        }

        [Test]
        public void CreatePropertyTest()
        {
            var actionResult = PropertyController.CreateProperty(FakeProperty).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Created);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Property>().IdProperty, FakeProperty.IdProperty);
        }

        [Test]
        public void UpdatePropertyTest()
        {
            var actionResult = PropertyController.UpdateProperty(FakeProperty).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Property>().IdProperty, FakeProperty.IdProperty);
        }

        [Test]
        public void DeletePropertyTest()
        {
            var actionResult = PropertyController.DeleteProperty(FakePropertyId).Result;
            //Assert
            Assert.AreEqual(((NoContentResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NoContent);
        }

        private static Property GetPropertyFake(Guid fakePropertyId)
            => new()
            {
                IdProperty = fakePropertyId,
                Name = "La Casa",
                Address = "Calle falsa 123",
                Price = 1000000.99m,
                CodeInternal = 1,
                Year = 2021,
                Owner = new()
                {
                    IdOwner = Guid.Parse("b5b1a0c6-efc7-43b4-91b1-024a0268a7cf"),
                    Name = "Jonathan Jimenez",
                    Address = "Calle falsa 123",
                    Photo = null,
                    Birthday = DateTime.Now
                }
            };

    }
}