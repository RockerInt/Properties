using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private Guid FakeNotFoundPropertyId;
        private Guid FakeExistPropertyId;
        private Property FakeProperty;
        private Property FakeNotFoundProperty;
        private Property FakeExistProperty;

        public PropertiesControllerTests()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _loggerMock = new Mock<ILogger<PropertiesController>>();
        }

        [SetUp]
        public void Setup()
        {
            FakePropertyId = Guid.Parse("8cc32b40-578d-47c1-bb9f-63240737243f");
            FakeNotFoundPropertyId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            FakeExistPropertyId = Guid.Parse("b5b1a0c6-efc7-43b4-91b1-024a0268a7cf");
            FakeProperty = GetPropertyFake(FakePropertyId);
            FakeNotFoundProperty = GetPropertyFake(FakeNotFoundPropertyId);
            FakeExistProperty = GetPropertyFake(FakeExistPropertyId);

            _propertyRepositoryMock.Setup(x => x.GetProperty(It.Is<Guid>(x => x == FakePropertyId)))
                .Returns(Task.FromResult(FakeProperty));
            _propertyRepositoryMock.Setup(x => x.GetProperty(It.Is<Guid>(x => x == FakeExistPropertyId)))
                .Returns(Task.FromResult(FakeExistProperty));
            _propertyRepositoryMock.Setup(x => x.GetProperties(It.IsAny<PropertiesParameters>()))
                .Returns(Task.FromResult(new List<Property>() { FakeProperty }));
            _propertyRepositoryMock.Setup(x => x.CreateProperty(It.Is<Property>(x => x.IdProperty == FakePropertyId)))
                .Returns(Task.FromResult(FakeProperty));
            _propertyRepositoryMock.Setup(x => x.UpdateProperty(It.Is<Property>(x => x.IdProperty == FakePropertyId)))
                .Returns(Task.FromResult(FakeProperty));
            _propertyRepositoryMock.Setup(x => x.DeleteProperty(It.Is<Property>(x => x.IdProperty == FakePropertyId)))
                .Returns(Task.FromResult(true));
            
            Property nullObj = null;
            _propertyRepositoryMock.Setup(x => x.GetProperty(It.Is<Guid>(x => x == FakeNotFoundPropertyId)))
                .Returns(Task.FromResult(nullObj));
            _propertyRepositoryMock.Setup(x => x.PropertyExists(It.Is<Guid>(x => x == FakeNotFoundPropertyId)))
                .Returns(false);
            _propertyRepositoryMock.Setup(x => x.PropertyExists(It.Is<Guid>(x => x == FakeExistPropertyId)))
                .Returns(true);
            _propertyRepositoryMock.Setup(x => x.CreateProperty(It.Is<Property>(x => x.IdProperty == FakeExistPropertyId)))
                .Throws(new DbUpdateException());
            _propertyRepositoryMock.Setup(x => x.UpdateProperty(It.Is<Property>(x => x.IdProperty == FakeNotFoundPropertyId)))
                .Throws(new DbUpdateConcurrencyException());
            _propertyRepositoryMock.Setup(x => x.DeleteProperty(It.Is<Property>(x => x.IdProperty == FakeExistPropertyId)))
                .Returns(Task.FromResult(false));


            PropertyController = new PropertiesController(
                _loggerMock.Object,
                _propertyRepositoryMock.Object
            );
        }

        #region Success Cases
        [Test]
        public void GetPropertyTest()
        {
            var actionResult = PropertyController.GetProperty(FakePropertyId).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Property>().IdProperty, FakeProperty.IdProperty);
        }

        [Test]
        public void GetPropertiesTest()
        {
            var actionResult = PropertyController.GetProperties(new PropertiesParameters()).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntityListSimple<Property>().FirstOrDefault().IdProperty, FakeProperty.IdProperty);
        }

        [Test]
        public void CreatePropertyTest()
        {
            var actionResult = PropertyController.CreateProperty(FakeProperty).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Created);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Property>().IdProperty, FakeProperty.IdProperty);
        }

        [Test]
        public void UpdatePropertyTest()
        {
            var actionResult = PropertyController.UpdateProperty(FakeProperty).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Property>().IdProperty, FakeProperty.IdProperty);
        }

        [Test]
        public void DeletePropertyTest()
        {
            var actionResult = PropertyController.DeleteProperty(FakePropertyId).Result;
            
            Assert.AreEqual(((NoContentResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NoContent);
        }
        #endregion

        #region Alternative Cases
        [Test]
        public void GetPropertyNotFoundTest()
        {
            var actionResult = PropertyController.GetProperty(FakeNotFoundPropertyId).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void CreatePropertyConflictTest()
        {
            var actionResult = PropertyController.CreateProperty(FakeExistProperty).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Conflict);
        }

        [Test]
        public void UpdatePropertyNotFoundTest()
        {
            var actionResult = PropertyController.UpdateProperty(FakeNotFoundProperty).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void DeletePropertyNotFoundTest()
        {
            var actionResult = PropertyController.DeleteProperty(FakeNotFoundPropertyId).Result;
            
            Assert.AreEqual(((NotFoundObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void DeletePropertyNotModifiedTest()
        {
            var actionResult = PropertyController.DeleteProperty(FakeExistPropertyId).Result;

            Assert.AreEqual(((StatusCodeResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotModified);
        }
        #endregion

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