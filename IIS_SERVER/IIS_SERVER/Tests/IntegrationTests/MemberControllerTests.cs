using IIS_SERVER.Services;
using IIS_SERVER.Member.Controllers;
using IIS_SERVER.Member.Models;
using IIS_SERVER.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;


namespace IIS_SERVER.Tests.IntegrationTests;

public class MemberControllerTests
{
    [TestFixture]
    public class UserControllerTests
    {
        [SetUp]
        public void Setup()
        {
            mySqlServiceMock = new Mock<IMySQLService>();
            controller = new MemberController(mySqlServiceMock.Object);
        }

        private MemberController controller;
        private Mock<IMySQLService> mySqlServiceMock;
        
        [Test]
        public async Task AddMember_MemberAdded_ReturnsCreatedResult()
        {
            // Arrange
            mySqlServiceMock.Setup(service => service.AddMember(It.IsAny<MemberModel>()))
                            .ReturnsAsync(Tuple.Create(true, (string?)null));

            // Act
            var member = new MemberModel();
            var result = await controller.AddMember(member) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
        }

        [Test]
        public async Task AddMember_MemberAlreadyInGroup_ReturnsConflictResult()
        {
            // Arrange
            mySqlServiceMock.Setup(service => service.AddMember(It.IsAny<MemberModel>()))
                            .ReturnsAsync(Tuple.Create(false, "exists."));

            // Act
            var member = new MemberModel();
            var result = await controller.AddMember(member) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(409, result.StatusCode);
            Assert.AreEqual("Error: Member already added in group.", result.Value);
        }

        [Test]
        public async Task AddMember_GroupNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock.Setup(service => service.AddMember(It.IsAny<MemberModel>()))
                            .ReturnsAsync(Tuple.Create(false, "Groups"));
            // Act
            var member = new MemberModel();
            var result = await controller.AddMember(member) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: Group not found.", result.Value);
        }

        [Test]
        public async Task AddMember_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock.Setup(service => service.AddMember(It.IsAny<MemberModel>()))
                            .ReturnsAsync(Tuple.Create(false, "Users"));

            // Act
            var member = new MemberModel();
            var result = await controller.AddMember(member) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: User not found.", result.Value);
        }

        [Test]
        public async Task DeleteMember_MemberDeleted_ReturnsNoContentResult()
        {
            // Arrange
            mySqlServiceMock.Setup(service => service.DeleteMember(It.IsAny<string>()))
                            .ReturnsAsync(true);

            // Act
            var result = await controller.DeleteMember("testMember") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task DeleteMember_MemberNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock.Setup(service => service.DeleteMember(It.IsAny<string>()))
                            .ReturnsAsync(false);

            // Act
            var result = await controller.DeleteMember("nonExistentMember") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: Member not found.", result.Value);
        }

        [Test]
        public async Task UpdateMemberRole_MemberRoleUpdated_ReturnsNoContentResult()
        {
            // Arrange
            mySqlServiceMock.Setup(service => service.UpdateMemberRole(It.IsAny<string>(), It.IsAny<GroupRole>()))
                            .ReturnsAsync(true);

            // Act
            var result = await controller.UpdateMemberRole("testMember", GroupRole.moderator) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task UpdateMemberRole_MemberNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock.Setup(service => service.UpdateMemberRole(It.IsAny<string>(), It.IsAny<GroupRole>()))
                            .ReturnsAsync(false);

            // Act
            var result = await controller.UpdateMemberRole("nonExistentMember", GroupRole.moderator) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: Member not found.", result.Value);
        }
    }
}
