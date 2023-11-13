using IIS_SERVER.Services;
using IIS_SERVER.User.Controllers;
using IIS_SERVER.User.Models;
using IIS_SERVER.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace UserControllerTests
{
    [TestFixture]
    public class UserControllerTests
    {
        [SetUp]
        public void Setup()
        {
            mySqlServiceMock = new Mock<IMySQLService>();
            controller = new UserController(mySqlServiceMock.Object);
        }

        private UserController controller;
        private Mock<IMySQLService> mySqlServiceMock;

        [Test]
        public async Task AddUser_ValidUser_ReturnsCreatedResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.AddUser(It.IsAny<UserDetailModel>()))
                .ReturnsAsync(Tuple.Create(true, (string)null));

            // Act
            var result = await controller.AddUser(new UserDetailModel()) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
        }

        [Test]
        public async Task AddUser_DuplicateUser_ReturnsConflictResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.AddUser(It.IsAny<UserDetailModel>()))
                .ReturnsAsync(Tuple.Create(false, "PRIMARY"));

            // Act
            var result = await controller.AddUser(new UserDetailModel()) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(409, result.StatusCode);
            Assert.AreEqual("Error: The user with email already exists.", result.Value);
        }

        [Test]
        public async Task AddUser_ErrorInService_ReturnsInternalServerErrorResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.AddUser(It.IsAny<UserDetailModel>()))
                .ReturnsAsync(Tuple.Create(false, "DB Error"));

            // Act
            var result = await controller.AddUser(new UserDetailModel()) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
        }

        [Test]
        public async Task GetUserRole_UserFound_ReturnsOkResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.GetUserRole(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create((Role?)Role.admin, ""));

            // Act
            var result = await controller.GetUserRole("testUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(Role.admin, result.Value);
        }

        [Test]
        public async Task GetUserRole_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.GetUserRole(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create((Role?)null, "Users"));
            // Act
            var result = await controller.GetUserRole("nonExistentUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: User not found.", result.Value);
        }

        [Test]
        public async Task UpdateUser_UserUpdated_ReturnsNoContentResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(
                    service =>
                        service.UpdateUser(
                            It.IsAny<UserDetailModel>(),
                            It.IsAny<UserPrivacySettingsModel>()
                        )
                )
                .ReturnsAsync(Tuple.Create(true, ""));
            // Act
            var updatedUser = new UpdateUserRequest();
            var result = await controller.UpdateUser(updatedUser) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task UpdateUser_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(
                    service =>
                        service.UpdateUser(
                            It.IsAny<UserDetailModel>(),
                            It.IsAny<UserPrivacySettingsModel>()
                        )
                )
                .ReturnsAsync(Tuple.Create(false, "Users"));

            // Act
            var updatedUser = new UpdateUserRequest();
            var result = await controller.UpdateUser(updatedUser) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: User not found.", result.Value);
        }

        [Test]
        public async Task DeleteUser_UserDeleted_ReturnsNoContentResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.DeleteUser(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create(true, (string?)null));

            // Act
            var result = await controller.DeleteUser("userToDelete") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task DeleteUser_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.DeleteUser(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create(false, "Users"));

            // Act
            var result = await controller.DeleteUser("nonExistentUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: User not found.", result.Value);
        }

        [Test]
        public async Task DeleteUser_UserIsAdminInGroup_ReturnsForbiddenResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.DeleteUser(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create(false, "group admin"));

            // Act
            var result = await controller.DeleteUser("adminUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(403, result.StatusCode);
            Assert.AreEqual(
                "Error: User cannot be deleted because is an admin in one or more groups.",
                result.Value
            );
        }

        [Test]
        public async Task DeleteUser_UserIsSystemAdmin_ReturnsForbiddenResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.DeleteUser(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create(false, "system admin"));

            // Act
            var result = await controller.DeleteUser("adminUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(403, result.StatusCode);
            Assert.AreEqual(
                "Error: User cannot be deleted because is an system admin.",
                result.Value
            );
        }

        [Test]
        public async Task GetUserPrivacySettings_SettingsFound_ReturnsOkResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.GetUserPrivacySettings(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create(new UserPrivacySettingsModel(), ""));

            // Act
            var result = await controller.GetUserPrivacySettings("testUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetUserPrivacySettings_SettingsNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.GetUserPrivacySettings(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create((UserPrivacySettingsModel?)null, "Users"));

            // Act
            var result = await controller.GetUserPrivacySettings("nonExistentUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: User not found.", result.Value);
        }

        [Test]
        public async Task GetUserProfile_UserFound_ReturnsOkResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.GetUserProfile(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create(new UserListModel(), ""));

            // Act
            var result = await controller.GetUserProfile("testUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetUserProfile_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            mySqlServiceMock
                .Setup(service => service.GetUserProfile(It.IsAny<string>()))
                .ReturnsAsync(Tuple.Create((UserListModel)null, "Users"));

            // Act
            var result = await controller.GetUserProfile("nonExistentUser") as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: User not found.", result.Value);
        }
    }
}
