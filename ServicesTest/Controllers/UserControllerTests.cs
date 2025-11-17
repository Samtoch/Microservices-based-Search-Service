using NUnit.Framework;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using UserService.Controllers;
using UserService.Services;
using UserService.Messaging;
//using UserService.Events;
using UserService.Data.Models;
using System.Threading.Tasks;

namespace UserService.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IEventPublisher> _eventPublisherMock;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _eventPublisherMock = new Mock<IEventPublisher>();

            _controller = new UserController(_userServiceMock.Object, _eventPublisherMock.Object);
        }

        // ✅ Test: GetUsers
        [Test]
        public async Task GetUsers_ShouldReturnOkWithUsers()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "John" },
                new User { Id = Guid.NewGuid(), FirstName = "Jane" }
            };

            var apiResponse = new ApiResponse<IEnumerable<User>>
            {
                Data = users,
                ResFlag = true,
                ResMsg = "Success"
            };

            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(apiResponse);

            var result = await _controller.GetUsers() as OkObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(apiResponse);
        }

        //// ✅ Test: GetUser(Guid id)
        //[Test]
        //public void GetUser_ShouldReturnOk_WhenUserExists()
        //{
        //    var userId = Guid.NewGuid();
        //    var user = new UserDto { Id = userId, Name = "John" };

        //    _userServiceMock.Setup(s => s.GetUser(userId)).Returns(user);

        //    var result = _controller.GetUser(userId) as OkObjectResult;

        //    result.Should().NotBeNull();
        //    result!.Value.Should().BeEquivalentTo(user);
        //}

        //[Test]
        //public void GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        //{
        //    _userServiceMock.Setup(s => s.GetUser(It.IsAny<Guid>())).Returns((UserDto)null);

        //    var result = _controller.GetUser(Guid.NewGuid());

        //    result.Should().BeOfType<NotFoundResult>();
        //}

        //// ✅ Test: CreateUser
        //[Test]
        //public void CreateUser_ShouldCallServiceAndPublishEvent()
        //{
        //    var dto = new UserDto { Id = Guid.NewGuid(), Name = "New User" };

        //    var result = _controller.CreateUser(dto, null) as CreatedResult;

        //    result.Should().NotBeNull();
        //    _userServiceMock.Verify(s => s.CreateUser(dto), Times.Once);
        //    _eventPublisherMock.Verify(e => e.Publish(It.IsAny<UserSignupEvent>()), Times.Once);
        //}

        //// ✅ Test: UpdateUser
        //[Test]
        //public void UpdateUser_ShouldCallService()
        //{
        //    var userId = Guid.NewGuid();
        //    var dto = new UserDto { Id = userId, Name = "Updated User" };

        //    var result = _controller.UpdateUser(userId, dto, null) as OkResult;

        //    result.Should().NotBeNull();
        //    _userServiceMock.Verify(s => s.UpdateUser(userId, dto), Times.Once);
        //}

        //// ✅ Test: DeleteUser
        //[Test]
        //public void DeleteUser_ShouldCallService()
        //{
        //    var userId = Guid.NewGuid();

        //    var result = _controller.DeleteUser(userId) as OkResult;

        //    result.Should().NotBeNull();
        //    _userServiceMock.Verify(s => s.DeleteUser(userId), Times.Once);
        //}

        //// ✅ Test: Signup
        //[Test]
        //public void Signup_ShouldPublishEvent()
        //{
        //    var dto = new UserDto { Id = Guid.NewGuid(), Name = "Signup User" };

        //    var result = _controller.Signup(dto) as OkResult;

        //    result.Should().NotBeNull();
        //    _eventPublisherMock.Verify(e => e.Publish(It.IsAny<UserSignupEvent>()), Times.Once);
        //}

        //// ✅ Test: PasswordReset
        //[Test]
        //public void PasswordReset_ShouldPublishEvent()
        //{
        //    var userId = Guid.NewGuid();

        //    var result = _controller.PasswordReset(userId) as OkResult;

        //    result.Should().NotBeNull();
        //    _eventPublisherMock.Verify(e => e.Publish(It.IsAny<PasswordResetEvent>()), Times.Once);
        //}
    }
}