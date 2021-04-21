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
    public class PropertyTracesControllerTests
    {
        private readonly Mock<IPropertyTraceRepository> _propertyTraceRepositoryMock;
        private readonly Mock<ILogger<PropertyTracesController>> _loggerMock;
        private PropertyTracesController PropertyTraceController;
        private Guid FakePropertyTraceId;
        private Guid FakeNotFoundPropertyTraceId;
        private Guid FakeExistPropertyTraceId;
        private PropertyTrace FakePropertyTrace;
        private PropertyTrace FakeNotFoundPropertyTrace;
        private PropertyTrace FakeExistPropertyTrace;

        public PropertyTracesControllerTests()
        {
            _propertyTraceRepositoryMock = new Mock<IPropertyTraceRepository>();
            _loggerMock = new Mock<ILogger<PropertyTracesController>>();
        }

        [SetUp]
        public void Setup()
        {
            FakePropertyTraceId = Guid.Parse("8cc32b40-578d-47c1-bb9f-63240737243f");
            FakeNotFoundPropertyTraceId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            FakeExistPropertyTraceId = Guid.Parse("b5b1a0c6-efc7-43b4-91b1-024a0268a7cf");
            FakePropertyTrace = GetPropertyTraceFake(FakePropertyTraceId);
            FakeNotFoundPropertyTrace = GetPropertyTraceFake(FakeNotFoundPropertyTraceId);
            FakeExistPropertyTrace = GetPropertyTraceFake(FakeExistPropertyTraceId);

            _propertyTraceRepositoryMock.Setup(x => x.GetPropertyTrace(It.Is<Guid>(x => x == FakePropertyTraceId)))
                .Returns(Task.FromResult(FakePropertyTrace));
            _propertyTraceRepositoryMock.Setup(x => x.GetPropertyTrace(It.Is<Guid>(x => x == FakeExistPropertyTraceId)))
                .Returns(Task.FromResult(FakeExistPropertyTrace));
            _propertyTraceRepositoryMock.Setup(x => x.GetPropertyTraces())
                .Returns(Task.FromResult(new List<PropertyTrace>() { FakePropertyTrace }));
            _propertyTraceRepositoryMock.Setup(x => x.CreatePropertyTrace(It.Is<PropertyTrace>(x => x.IdPropertyTrace == FakePropertyTraceId)))
                .Returns(Task.FromResult(FakePropertyTrace));
            _propertyTraceRepositoryMock.Setup(x => x.UpdatePropertyTrace(It.Is<PropertyTrace>(x => x.IdPropertyTrace == FakePropertyTraceId)))
                .Returns(Task.FromResult(FakePropertyTrace));
            _propertyTraceRepositoryMock.Setup(x => x.DeletePropertyTrace(It.Is<PropertyTrace>(x => x.IdPropertyTrace == FakePropertyTraceId)))
                .Returns(Task.FromResult(true));
            
            PropertyTrace nullObj = null;
            _propertyTraceRepositoryMock.Setup(x => x.GetPropertyTrace(It.Is<Guid>(x => x == FakeNotFoundPropertyTraceId)))
                .Returns(Task.FromResult(nullObj));
            _propertyTraceRepositoryMock.Setup(x => x.PropertyTraceExists(It.Is<Guid>(x => x == FakeNotFoundPropertyTraceId)))
                .Returns(false);
            _propertyTraceRepositoryMock.Setup(x => x.PropertyTraceExists(It.Is<Guid>(x => x == FakeExistPropertyTraceId)))
                .Returns(true);
            _propertyTraceRepositoryMock.Setup(x => x.CreatePropertyTrace(It.Is<PropertyTrace>(x => x.IdPropertyTrace == FakeExistPropertyTraceId)))
                .Throws(new DbUpdateException());
            _propertyTraceRepositoryMock.Setup(x => x.UpdatePropertyTrace(It.Is<PropertyTrace>(x => x.IdPropertyTrace == FakeNotFoundPropertyTraceId)))
                .Throws(new DbUpdateConcurrencyException());
            _propertyTraceRepositoryMock.Setup(x => x.DeletePropertyTrace(It.Is<PropertyTrace>(x => x.IdPropertyTrace == FakeExistPropertyTraceId)))
                .Returns(Task.FromResult(false));


            PropertyTraceController = new PropertyTracesController(
                _loggerMock.Object,
                _propertyTraceRepositoryMock.Object
            );
        }

        #region Success Cases
        [Test]
        public void GetPropertyTraceTest()
        {
            var actionResult = PropertyTraceController.GetPropertyTrace(FakePropertyTraceId).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyTrace>().IdPropertyTrace, FakePropertyTrace.IdPropertyTrace);
        }

        [Test]
        public void GetPropertyTracesTest()
        {
            var actionResult = PropertyTraceController.GetPropertyTraces().Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntityListSimple<PropertyTrace>().FirstOrDefault().IdPropertyTrace, FakePropertyTrace.IdPropertyTrace);
        }

        [Test]
        public void CreatePropertyTraceTest()
        {
            var actionResult = PropertyTraceController.CreatePropertyTrace(FakePropertyTrace).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Created);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyTrace>().IdPropertyTrace, FakePropertyTrace.IdPropertyTrace);
        }

        [Test]
        public void UpdatePropertyTraceTest()
        {
            var actionResult = PropertyTraceController.UpdatePropertyTrace(FakePropertyTrace).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyTrace>().IdPropertyTrace, FakePropertyTrace.IdPropertyTrace);
        }

        [Test]
        public void DeletePropertyTraceTest()
        {
            var actionResult = PropertyTraceController.DeletePropertyTrace(FakePropertyTraceId).Result;
            
            Assert.AreEqual(((NoContentResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NoContent);
        }
        #endregion

        #region Alternative Cases
        [Test]
        public void GetPropertyTraceNotFoundTest()
        {
            var actionResult = PropertyTraceController.GetPropertyTrace(FakeNotFoundPropertyTraceId).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void CreatePropertyTraceConflictTest()
        {
            var actionResult = PropertyTraceController.CreatePropertyTrace(FakeExistPropertyTrace).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Conflict);
        }

        [Test]
        public void UpdatePropertyTraceNotFoundTest()
        {
            var actionResult = PropertyTraceController.UpdatePropertyTrace(FakeNotFoundPropertyTrace).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void DeletePropertyTraceNotFoundTest()
        {
            var actionResult = PropertyTraceController.DeletePropertyTrace(FakeNotFoundPropertyTraceId).Result;
            
            Assert.AreEqual(((NotFoundObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void DeletePropertyTraceNotModifiedTest()
        {
            var actionResult = PropertyTraceController.DeletePropertyTrace(FakeExistPropertyTraceId).Result;

            Assert.AreEqual(((StatusCodeResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotModified);
        }
        #endregion

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