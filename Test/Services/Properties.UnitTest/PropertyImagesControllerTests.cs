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
    public class PropertyImagesControllerTests
    {
        private readonly Mock<IPropertyImageRepository> _propertyImageRepositoryMock;
        private readonly Mock<ILogger<PropertyImagesController>> _loggerMock;
        private PropertyImagesController PropertyImageController;
        private Guid FakePropertyImageId;
        private Guid FakeNotFoundPropertyImageId;
        private Guid FakeExistPropertyImageId;
        private PropertyImage FakePropertyImage;
        private PropertyImage FakeNotFoundPropertyImage;
        private PropertyImage FakeExistPropertyImage;

        public PropertyImagesControllerTests()
        {
            _propertyImageRepositoryMock = new Mock<IPropertyImageRepository>();
            _loggerMock = new Mock<ILogger<PropertyImagesController>>();
        }

        [SetUp]
        public void Setup()
        {
            FakePropertyImageId = Guid.Parse("8cc32b40-578d-47c1-bb9f-63240737243f");
            FakeNotFoundPropertyImageId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            FakeExistPropertyImageId = Guid.Parse("b5b1a0c6-efc7-43b4-91b1-024a0268a7cf");
            FakePropertyImage = GetPropertyImageFake(FakePropertyImageId);
            FakeNotFoundPropertyImage = GetPropertyImageFake(FakeNotFoundPropertyImageId);
            FakeExistPropertyImage = GetPropertyImageFake(FakeExistPropertyImageId);

            _propertyImageRepositoryMock.Setup(x => x.GetPropertyImage(It.Is<Guid>(x => x == FakePropertyImageId)))
                .Returns(Task.FromResult(FakePropertyImage));
            _propertyImageRepositoryMock.Setup(x => x.GetPropertyImage(It.Is<Guid>(x => x == FakeExistPropertyImageId)))
                .Returns(Task.FromResult(FakeExistPropertyImage));
            _propertyImageRepositoryMock.Setup(x => x.GetPropertyImages())
                .Returns(Task.FromResult(new List<PropertyImage>() { FakePropertyImage }));
            _propertyImageRepositoryMock.Setup(x => x.CreatePropertyImage(It.Is<PropertyImage>(x => x.IdPropertyImage == FakePropertyImageId)))
                .Returns(Task.FromResult(FakePropertyImage));
            _propertyImageRepositoryMock.Setup(x => x.UpdatePropertyImage(It.Is<PropertyImage>(x => x.IdPropertyImage == FakePropertyImageId)))
                .Returns(Task.FromResult(FakePropertyImage));
            _propertyImageRepositoryMock.Setup(x => x.DeletePropertyImage(It.Is<PropertyImage>(x => x.IdPropertyImage == FakePropertyImageId)))
                .Returns(Task.FromResult(true));
            
            PropertyImage nullObj = null;
            _propertyImageRepositoryMock.Setup(x => x.GetPropertyImage(It.Is<Guid>(x => x == FakeNotFoundPropertyImageId)))
                .Returns(Task.FromResult(nullObj));
            _propertyImageRepositoryMock.Setup(x => x.PropertyImageExists(It.Is<Guid>(x => x == FakeNotFoundPropertyImageId)))
                .Returns(false);
            _propertyImageRepositoryMock.Setup(x => x.PropertyImageExists(It.Is<Guid>(x => x == FakeExistPropertyImageId)))
                .Returns(true);
            _propertyImageRepositoryMock.Setup(x => x.CreatePropertyImage(It.Is<PropertyImage>(x => x.IdPropertyImage == FakeExistPropertyImageId)))
                .Throws(new DbUpdateException());
            _propertyImageRepositoryMock.Setup(x => x.UpdatePropertyImage(It.Is<PropertyImage>(x => x.IdPropertyImage == FakeNotFoundPropertyImageId)))
                .Throws(new DbUpdateConcurrencyException());
            _propertyImageRepositoryMock.Setup(x => x.DeletePropertyImage(It.Is<PropertyImage>(x => x.IdPropertyImage == FakeExistPropertyImageId)))
                .Returns(Task.FromResult(false));


            PropertyImageController = new PropertyImagesController(
                _loggerMock.Object,
                _propertyImageRepositoryMock.Object
            );
        }

        #region Success Cases
        [Test]
        public void GetPropertyImageTest()
        {
            var actionResult = PropertyImageController.GetPropertyImage(FakePropertyImageId).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyImage>().IdPropertyImage, FakePropertyImage.IdPropertyImage);
        }

        [Test]
        public void GetPropertyImagesTest()
        {
            var actionResult = PropertyImageController.GetPropertyImages().Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntityListSimple<PropertyImage>().FirstOrDefault().IdPropertyImage, FakePropertyImage.IdPropertyImage);
        }

        [Test]
        public void CreatePropertyImageTest()
        {
            var actionResult = PropertyImageController.CreatePropertyImage(FakePropertyImage).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Created);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyImage>().IdPropertyImage, FakePropertyImage.IdPropertyImage);
        }

        [Test]
        public void UpdatePropertyImageTest()
        {
            var actionResult = PropertyImageController.UpdatePropertyImage(FakePropertyImage).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyImage>().IdPropertyImage, FakePropertyImage.IdPropertyImage);
        }

        [Test]
        public void DeletePropertyImageTest()
        {
            var actionResult = PropertyImageController.DeletePropertyImage(FakePropertyImageId).Result;
            
            Assert.AreEqual(((NoContentResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NoContent);
        }
        #endregion

        #region Alternative Cases
        [Test]
        public void GetPropertyImageNotFoundTest()
        {
            var actionResult = PropertyImageController.GetPropertyImage(FakeNotFoundPropertyImageId).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void CreatePropertyImageConflictTest()
        {
            var actionResult = PropertyImageController.CreatePropertyImage(FakeExistPropertyImage).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Conflict);
        }

        [Test]
        public void UpdatePropertyImageNotFoundTest()
        {
            var actionResult = PropertyImageController.UpdatePropertyImage(FakeNotFoundPropertyImage).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void DeletePropertyImageNotFoundTest()
        {
            var actionResult = PropertyImageController.DeletePropertyImage(FakeNotFoundPropertyImageId).Result;
            
            Assert.AreEqual(((NotFoundObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void DeletePropertyImageNotModifiedTest()
        {
            var actionResult = PropertyImageController.DeletePropertyImage(FakeExistPropertyImageId).Result;

            Assert.AreEqual(((StatusCodeResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotModified);
        }
        #endregion

        private static PropertyImage GetPropertyImageFake(Guid fakePropertyImageId)
            => new()
            {
                IdPropertyImage = fakePropertyImageId,
                Enabled = true
            };

    }
}