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
    public class OwnersControllerTests
    {
        private readonly Mock<IOwnerRepository> _ownerRepositoryMock;
        private readonly Mock<ILogger<OwnersController>> _loggerMock;
        private OwnersController OwnerController;
        private Guid FakeOwnerId;
        private Owner FakeOwner;

        public OwnersControllerTests()
        {
            _ownerRepositoryMock = new Mock<IOwnerRepository>();
            _loggerMock = new Mock<ILogger<OwnersController>>();
        }

        [SetUp]
        public void Setup()
        {
            FakeOwnerId = Guid.Parse("8cc32b40-578d-47c1-bb9f-63240737243f");
            FakeOwner = GetOwnerFake(FakeOwnerId);

            _ownerRepositoryMock.Setup(x => x.GetOwner(It.IsAny<Guid>()))
                .Returns(Task.FromResult(FakeOwner));

            _ownerRepositoryMock.Setup(x => x.GetOwners())
                .Returns(Task.FromResult(new List<Owner>() { FakeOwner }));

            _ownerRepositoryMock.Setup(x => x.CreateOwner(It.IsAny<Owner>()))
                .Returns(Task.FromResult(FakeOwner));

            _ownerRepositoryMock.Setup(x => x.UpdateOwner(It.IsAny<Owner>()))
                .Returns(Task.FromResult(FakeOwner));

            _ownerRepositoryMock.Setup(x => x.DeleteOwner(It.IsAny<Owner>()))
                .Returns(Task.FromResult(true));


            OwnerController = new OwnersController(
                _loggerMock.Object,
                _ownerRepositoryMock.Object
            );
        }

        [Test]
        public void GetOwnerTest()
        {
            var actionResult = OwnerController.GetOwner(FakeOwnerId).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Owner>().IdOwner, FakeOwner.IdOwner);
        }

        [Test]
        public void GetOwnersTest()
        {
            var actionResult = OwnerController.GetOwners().Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntityListSimple<Owner>().FirstOrDefault().IdOwner, FakeOwner.IdOwner);
        }

        [Test]
        public void CreateOwnerTest()
        {
            var actionResult = OwnerController.CreateOwner(FakeOwner).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Created);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Owner>().IdOwner, FakeOwner.IdOwner);
        }

        [Test]
        public void UpdateOwnerTest()
        {
            var actionResult = OwnerController.UpdateOwner(FakeOwner).Result;
            //Assert
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Owner>().IdOwner, FakeOwner.IdOwner);
        }

        [Test]
        public void DeleteOwnerTest()
        {
            var actionResult = OwnerController.DeleteOwner(FakeOwnerId).Result;
            //Assert
            Assert.AreEqual(((NoContentResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NoContent);
        }

        private static Owner GetOwnerFake(Guid fakeOwnerId)
            => new()
            {
                IdOwner = fakeOwnerId,
                Name = "Jonathan Jimenez",
                Address = "Calle falsa 123",
                Photo = null,
                Birthday = DateTime.Now
            };

    }
}