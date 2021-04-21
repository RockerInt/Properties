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
    public class PropertyImagesControllerTests
    {
        private readonly Mock<IPropertyImageRepository> _propertyImageRepositoryMock;
        private readonly Mock<ILogger<PropertyImagesController>> _loggerMock;
        private PropertyImagesController PropertyImageController;
        private Guid FakePropertyImageId;
        private PropertyImage FakePropertyImage;

        public PropertyImagesControllerTests()
        {
            _propertyImageRepositoryMock = new Mock<IPropertyImageRepository>();
            _loggerMock = new Mock<ILogger<PropertyImagesController>>();
        }

        [SetUp]
        public void Setup()
        {
            FakePropertyImageId = Guid.Parse("8cc32b40-578d-47c1-bb9f-63240737243f");
            FakePropertyImage = GetPropertyImageFake(FakePropertyImageId);

            _propertyImageRepositoryMock.Setup(x => x.GetPropertyImage(It.IsAny<Guid>()))
                .Returns(Task.FromResult(FakePropertyImage));

            _propertyImageRepositoryMock.Setup(x => x.GetPropertyImages())
                .Returns(Task.FromResult(new List<PropertyImage>() { FakePropertyImage }));

            _propertyImageRepositoryMock.Setup(x => x.CreatePropertyImage(It.IsAny<PropertyImage>()))
                .Returns(Task.FromResult(FakePropertyImage));

            _propertyImageRepositoryMock.Setup(x => x.UpdatePropertyImage(It.IsAny<PropertyImage>()))
                .Returns(Task.FromResult(FakePropertyImage));

            _propertyImageRepositoryMock.Setup(x => x.DeletePropertyImage(It.IsAny<PropertyImage>()))
                .Returns(Task.FromResult(true));


            PropertyImageController = new PropertyImagesController(
                _loggerMock.Object,
                _propertyImageRepositoryMock.Object
            );
        }

        [Test]
        public void GetPropertyImageTest()
        {
            var actionResult = PropertyImageController.GetPropertyImage(FakePropertyImageId).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyImage>().IdPropertyImage, FakePropertyImage.IdPropertyImage);
        }

        [Test]
        public void GetPropertyImagesTest()
        {
            var actionResult = PropertyImageController.GetPropertyImages().Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntityListSimple<PropertyImage>().FirstOrDefault().IdPropertyImage, FakePropertyImage.IdPropertyImage);
        }

        [Test]
        public void CreatePropertyImageTest()
        {
            var actionResult = PropertyImageController.CreatePropertyImage(FakePropertyImage).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Created);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyImage>().IdPropertyImage, FakePropertyImage.IdPropertyImage);
        }

        [Test]
        public void UpdatePropertyImageTest()
        {
            var actionResult = PropertyImageController.UpdatePropertyImage(FakePropertyImage).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<PropertyImage>().IdPropertyImage, FakePropertyImage.IdPropertyImage);
        }

        [Test]
        public void DeletePropertyImageTest()
        {
            var actionResult = PropertyImageController.DeletePropertyImage(FakePropertyImageId).Result;
            //Assert
            Assert.AreEqual(((NoContentResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NoContent);
        }

        private static PropertyImage GetPropertyImageFake(Guid fakePropertyImageId)
            => new()
            {
                IdPropertyImage = fakePropertyImageId,
                Enabled = true
            };

    }
}