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
    public class OwnersControllerTests
    {
        private readonly Mock<IOwnerRepository> _ownerRepositoryMock;
        private readonly Mock<ILogger<OwnersController>> _loggerMock;
        private OwnersController OwnerController;
        private Guid FakeOwnerId;
        private Guid FakeNotFoundOwnerId;
        private Guid FakeExistOwnerId;
        private Owner FakeOwner;
        private Owner FakeNotFoundOwner;
        private Owner FakeExistOwner;

        public OwnersControllerTests()
        {
            _ownerRepositoryMock = new Mock<IOwnerRepository>();
            _loggerMock = new Mock<ILogger<OwnersController>>();
        }

        [SetUp]
        public void Setup()
        {
            FakeOwnerId = Guid.Parse("8cc32b40-578d-47c1-bb9f-63240737243f");
            FakeNotFoundOwnerId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            FakeExistOwnerId = Guid.Parse("b5b1a0c6-efc7-43b4-91b1-024a0268a7cf");
            FakeOwner = GetOwnerFake(FakeOwnerId);
            FakeNotFoundOwner = GetOwnerFake(FakeNotFoundOwnerId);
            FakeExistOwner = GetOwnerFake(FakeExistOwnerId);

            _ownerRepositoryMock.Setup(x => x.GetOwner(It.Is<Guid>(x => x == FakeOwnerId)))
                .Returns(Task.FromResult(FakeOwner));
            _ownerRepositoryMock.Setup(x => x.GetOwner(It.Is<Guid>(x => x == FakeExistOwnerId)))
                .Returns(Task.FromResult(FakeExistOwner));
            _ownerRepositoryMock.Setup(x => x.GetOwners())
                .Returns(Task.FromResult(new List<Owner>() { FakeOwner }));
            _ownerRepositoryMock.Setup(x => x.CreateOwner(It.Is<Owner>(x => x.IdOwner == FakeOwnerId)))
                .Returns(Task.FromResult(FakeOwner));
            _ownerRepositoryMock.Setup(x => x.UpdateOwner(It.Is<Owner>(x => x.IdOwner == FakeOwnerId)))
                .Returns(Task.FromResult(FakeOwner));
            _ownerRepositoryMock.Setup(x => x.DeleteOwner(It.Is<Owner>(x => x.IdOwner == FakeOwnerId)))
                .Returns(Task.FromResult(true));
            
            Owner nullObj = null;
            _ownerRepositoryMock.Setup(x => x.GetOwner(It.Is<Guid>(x => x == FakeNotFoundOwnerId)))
                .Returns(Task.FromResult(nullObj));
            _ownerRepositoryMock.Setup(x => x.OwnerExists(It.Is<Guid>(x => x == FakeNotFoundOwnerId)))
                .Returns(false);
            _ownerRepositoryMock.Setup(x => x.OwnerExists(It.Is<Guid>(x => x == FakeExistOwnerId)))
                .Returns(true);
            _ownerRepositoryMock.Setup(x => x.CreateOwner(It.Is<Owner>(x => x.IdOwner == FakeExistOwnerId)))
                .Throws(new DbUpdateException());
            _ownerRepositoryMock.Setup(x => x.UpdateOwner(It.Is<Owner>(x => x.IdOwner == FakeNotFoundOwnerId)))
                .Throws(new DbUpdateConcurrencyException());
            _ownerRepositoryMock.Setup(x => x.DeleteOwner(It.Is<Owner>(x => x.IdOwner == FakeExistOwnerId)))
                .Returns(Task.FromResult(false));


            OwnerController = new OwnersController(
                _loggerMock.Object,
                _ownerRepositoryMock.Object
            );
        }

        #region Success Cases
        [Test]
        public void GetOwnerTest()
        {
            var actionResult = OwnerController.GetOwner(FakeOwnerId).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Owner>().IdOwner, FakeOwner.IdOwner);
        }

        [Test]
        public void GetOwnersTest()
        {
            var actionResult = OwnerController.GetOwners().Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntityListSimple<Owner>().FirstOrDefault().IdOwner, FakeOwner.IdOwner);
        }

        [Test]
        public void CreateOwnerTest()
        {
            var actionResult = OwnerController.CreateOwner(FakeOwner).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Created);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Owner>().IdOwner, FakeOwner.IdOwner);
        }

        [Test]
        public void UpdateOwnerTest()
        {
            var actionResult = OwnerController.UpdateOwner(FakeOwner).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.AreEqual(((ObjectResult)actionResult).Value.ToString().ToEntitySimple<Owner>().IdOwner, FakeOwner.IdOwner);
        }

        [Test]
        public void DeleteOwnerTest()
        {
            var actionResult = OwnerController.DeleteOwner(FakeOwnerId).Result;
            
            Assert.AreEqual(((NoContentResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NoContent);
        }
        #endregion

        #region Alternative Cases
        [Test]
        public void GetOwnerNotFoundTest()
        {
            var actionResult = OwnerController.GetOwner(FakeNotFoundOwnerId).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void CreateOwnerConflictTest()
        {
            var actionResult = OwnerController.CreateOwner(FakeExistOwner).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.Conflict);
        }

        [Test]
        public void UpdateOwnerNotFoundTest()
        {
            var actionResult = OwnerController.UpdateOwner(FakeNotFoundOwner).Result;
            
            Assert.AreEqual(((ObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void DeleteOwnerNotFoundTest()
        {
            var actionResult = OwnerController.DeleteOwner(FakeNotFoundOwnerId).Result;
            
            Assert.AreEqual(((NotFoundObjectResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotFound);
        }

        [Test]
        public void DeleteOwnerNotModifiedTest()
        {
            var actionResult = OwnerController.DeleteOwner(FakeExistOwnerId).Result;

            Assert.AreEqual(((StatusCodeResult)actionResult).StatusCode, (int)System.Net.HttpStatusCode.NotModified);
        }
        #endregion

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