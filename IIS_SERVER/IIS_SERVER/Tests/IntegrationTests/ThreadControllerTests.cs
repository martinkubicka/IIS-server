using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IIS_SERVER.Services;
using IIS_SERVER.Thread.Controllers;
using IIS_SERVER.Thread.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace IIS_SERVER.Thread.Tests
{
    [TestFixture]
    public class ThreadControllerTests
    {
        private ThreadController _controller;
        private Mock<IMySQLService> _mySqlServiceMock;

        [SetUp]
        public void Setup()
        {
            _mySqlServiceMock = new Mock<IMySQLService>();
            _controller = new ThreadController(_mySqlServiceMock.Object);
        }

        [Test]
        public async Task CreateThread_ValidThread_Returns201StatusCode()
        {
            // Arrange
            var thread = new ThreadModel { };
            _mySqlServiceMock.Setup(service => service.CreateThread(thread))
                .ReturnsAsync(Tuple.Create(true, (string?)null));

            // Act
            var result = await _controller.CreateThread(thread) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.AreEqual("Thread successfully added to DB.", result.Value);
        }

        [Test]
        public async Task CreateThread_DuplicateThread_Returns500StatusCode()
        {
            // Arrange
            var thread = new ThreadModel { };
            _mySqlServiceMock.Setup(service => service.CreateThread(thread))
                .ReturnsAsync(Tuple.Create(false, "Error: The thread with id already exists."));

            // Act
            var result = await _controller.CreateThread(thread) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(500, result.StatusCode);
            Assert.AreEqual("Error: The thread with id already exists.", result.Value as string);
        }

        [Test]
        public async Task GetAllThreads_ValidThreads_Returns200StatusCode()
        {
            // Arrange
            var threads = new List<ThreadModel> { };
            _mySqlServiceMock.Setup(service => service.GetAllThreads())
                .ReturnsAsync(threads);

            // Act
            var result = await _controller.GetAllThreads() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(threads, result.Value);
        }

        [Test]
        public async Task GetThread_ExistingThreadId_ReturnsThread()
        {
            // Arrange
            var threadId = Guid.NewGuid(); 
            var thread = new ThreadModel { };
            _mySqlServiceMock.Setup(service => service.GetThread(threadId))
                .ReturnsAsync(thread);

            // Act
            var result = await _controller.GetThread(threadId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(thread, result.Value);
        }

        [Test]
        public async Task GetThread_NonExistingThreadId_Returns404StatusCode()
        {
            // Arrange
            var threadId = Guid.NewGuid(); 
            _mySqlServiceMock.Setup(service => service.GetThread(threadId))
                .ReturnsAsync((ThreadModel)null);

            // Act
            var result = await _controller.GetThread(threadId) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: Thread not found.", result.Value);
        }

        [Test]
        public async Task UpdateThread_ExistingThread_Returns200StatusCode()
        {
            // Arrange
            var threadId = Guid.NewGuid(); 
            var updatedThread = new ThreadModel
            {
                Name = "Updated Thread Name"
            };

            _mySqlServiceMock.Setup(service => service.UpdateThread(threadId, updatedThread))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateThread(threadId, updatedThread) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Thread successfully updated.", result.Value);
        }

        [Test]
        public async Task UpdateThread_NonExistingThread_Returns404StatusCode()
        {
            // Arrange
            var threadId = Guid.NewGuid(); 
            var updatedThread = new ThreadModel
            {
                Name = "Updated Thread Name",
            };

            _mySqlServiceMock.Setup(service => service.UpdateThread(threadId, updatedThread))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateThread(threadId, updatedThread) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: Thread not found or DB error occurred.", result.Value);
        }

        [Test]
        public async Task DeleteThread_ValidThreadId_Success()
        {
            // Arrange
            var validThreadId = Guid.NewGuid(); 
            _mySqlServiceMock.Setup(service => service.DeleteThread(validThreadId))
                .ReturnsAsync(Tuple.Create(true, ""));

            // Act
            var result = await _controller.DeleteThread(validThreadId);

            // Assert
            Console.WriteLine(result);
            Assert.AreEqual(204, (result as ObjectResult)?.StatusCode);
        }

        [Test]
        public async Task DeleteThread_InvalidThreadId_NotFound()
        {
            // Arrange
            var invalidThreadId = Guid.NewGuid(); 
            _mySqlServiceMock.Setup(service => service.DeleteThread(invalidThreadId))
                .ReturnsAsync(Tuple.Create(false, "Error: Thread not found."));

            // Act
            var result = await _controller.DeleteThread(invalidThreadId);

            // Assert
            Assert.AreEqual(404, (result as ObjectResult)?.StatusCode);
        }

        [Test]
        public async Task DeleteThread_Exception_Error()
        {
            // Arrange
            var exceptionMessage = "An error occurred.";
            var threadId = Guid.NewGuid(); 
            _mySqlServiceMock.Setup(service => service.DeleteThread(threadId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.DeleteThread(threadId);

            // Assert
            Assert.AreEqual(500, (result as ObjectResult)?.StatusCode);
        }

        [Test]
        public async Task GetThreadsFromSpecificGroup_ValidGroupName_Returns200StatusCode()
        {
            // Arrange
            var groupName = "group1";
            var threads = new List<ThreadModel> { }; 
            _mySqlServiceMock.Setup(service => service.GetThreadsFromSpecificGroup(groupName, 10, 10))
                .ReturnsAsync(threads);

            // Act
            var result = await _controller.GetThreadsFromSpecificGroup(groupName, 10, 10) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(threads, result.Value);
        }

        [Test]
        public async Task GetThreadsFromSpecificGroup_InvalidGroupName_Returns404StatusCode()
        {
            // Arrange
            var invalidGroupName = "nonexistentgroup";
            _mySqlServiceMock.Setup(service => service.GetThreadsFromSpecificGroup(invalidGroupName, 10, 10))
                .ReturnsAsync((List<ThreadModel>)null);

            // Act
            var result = await _controller.GetThreadsFromSpecificGroup(invalidGroupName, 10, 10) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Error: Thread not found.", result.Value);
        }

        [Test]
        public async Task GetThreadsFromSpecificGroup_Exception_Error()
        {
            // Arrange
            var exceptionMessage = "An error occurred.";
            var groupName = "group1";
            _mySqlServiceMock.Setup(service => service.GetThreadsFromSpecificGroup(groupName, 10, 10))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetThreadsFromSpecificGroup(groupName, 10, 10);

            // Assert
            Assert.AreEqual(500, (result as ObjectResult)?.StatusCode);
        }
    }
}
