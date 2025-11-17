using NUnit.Framework;
using Moq;
using FluentAssertions;
using UserService.Services;
using UserService.Repositories;
using UserService.DTOs;
using System;

namespace UserService.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        //private Mock<IRepository<UserDto>> _repoMock;
        //private UserService.Services.UserService _service;

        //[SetUp]
        //public void Setup()
        //{
        //    _repoMock = new Mock<IRepository<UserDto>>();
        //    _service = new UserService.Services.UserService(_repoMock.Object);
        //}

        //[Test]
        //public void CreateUser_ShouldAddUser()
        //{
        //    var user = new UserDto { Id = Guid.NewGuid(), Name = "Jane" };

        //    _service.CreateUser(user);

        //    _repoMock.Verify(r => r.Add(user), Times.Once);
        //}
    }
}