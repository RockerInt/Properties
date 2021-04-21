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
    public class PropertyTracesControllerTests
    {
        private readonly Mock<IPropertyTraceRepository> _propertyTraceRepositoryMock;
        private readonly Mock<ILogger<PropertyTracesController>> _loggerMock;
        private PropertyTracesController PropertyTraceController;
        private Guid FakePropertyTraceId;
        private PropertyTrace FakePropertyTrace;

        public PropertyTracesControllerTests()
        {
            _propertyTraceRepositoryMock = new Mock<IPropertyTraceRepository>();
            _loggerMock = new Mock<ILogger<PropertyTracesController>>();
        }

        [SetUp]
        public void Setup()
        {
            FakePropertyTraceId = Guid.Parse("8cc32b40-578d-47c1-bb9f-63240737243f");
            FakePropertyTrace = GetPropertyTraceFake(FakePropertyTraceId);

            _propertyTraceRepositoryMock.Setup(x => x.GetPropertyTrace(It.IsAny<Guid>()))
                .Returns(Task.FromResult(FakePropertyTrace));

            _propertyTraceRepositoryMock.Setup(x => x.GetPropertyTraces())
                .Returns(Task.FromResult(new List<PropertyTrace>() { FakePropertyTrace }));

            _propertyTraceRepositoryMock.Setup(x => x.CreatePropertyTrace(It.IsAny<PropertyTrace>()))
                .Returns(Task.FromResult(FakePropertyTrace));

            _propertyTraceRepositoryMock.Setup(x => x.UpdatePropertyTrace(It.IsAny<PropertyTrace>()))
                .Returns(Task.FromResult(FakePropertyTrace));

            _propertyTraceRepositoryMock.Setup(x => x.DeletePropertyTrace(It.IsAny<PropertyTrace>()))
                .Returns(Task.FromResult(true));


            PropertyTraceController = new PropertyTracesController(
                _loggerMock.Object,
                _propertyTraceRepositoryMock.Object
            );
        }

        [Test]
        public void GetPropertyTraceTest()
        {
            var actionResult = PropertyTraceController.GetPropertyTrace(FakePropertyTraceId).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyTrace>().IdPropertyTrace, FakePropertyTrace.IdPropertyTrace);
        }

        [Test]
        public void GetPropertyTracesTest()
        {
            var actionResult = PropertyTraceController.GetPropertyTraces().Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntityListSimple<PropertyTrace>().FirstOrDefault().IdPropertyTrace, FakePropertyTrace.IdPropertyTrace);
        }

        [Test]
        public void CreatePropertyTraceTest()
        {
            var actionResult = PropertyTraceController.CreatePropertyTrace(FakePropertyTrace).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Created);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyTrace>().IdPropertyTrace, FakePropertyTrace.IdPropertyTrace);
        }

        [Test]
        public void UpdatePropertyTraceTest()
        {
            var actionResult = PropertyTraceController.UpdatePropertyTrace(FakePropertyTrace).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyTrace>().IdPropertyTrace, FakePropertyTrace.IdPropertyTrace);
        }

        [Test]
        public void DeletePropertyTraceTest()
        {
            var actionResult = PropertyTraceController.DeletePropertyTrace(FakePropertyTraceId).Result;
            //Assert
            Assert.AreEqual(((NoContentResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NoContent);
        }

        private static PropertyTrace GetPropertyTraceFake(Guid fakePropertyTraceId)
            => new()
            {
                IdPropertyTrace = fakePropertyTraceId,
                Name = "Jonathan Jimenez",
                DateSale = DateTime.Now,
                Tax = 100000m,
                Value = 1200000m
            };

    }
}