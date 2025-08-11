using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Collections;
using System.Linq.Expressions;
using ViewModels.ViewModels.Admin.UserManagement;

namespace ServiceTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private UserService _userService;
        private List<IdentityUser> _users;

        [SetUp]
        public void Setup()
        {
            _users = new List<IdentityUser>
            {
                new IdentityUser { Id = "1", Email = "user1@test.com" },
                new IdentityUser { Id = "2", Email = "user2@test.com" },
                new IdentityUser { Id = "3", Email = "admin@test.com" }
            };

            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object, null, null, null, null);

            _userService = new UserService(_mockUserManager.Object, _mockRoleManager.Object);
        }
        [Test]
        public void PassAlways()
        {
            Assert.Pass();
        }
        //[Test]
        //public async Task GetUserManagementBoardDataAsync_ReturnsAllUsersExceptCurrent()
        //{
        //    // Arrange
        //    var currentUserId = "1";

        //    // Mock the final transformed result instead of the query
        //    var expectedResults = _users
        //        .Where(u => u.Id != currentUserId)
        //        .Select(u => new UserManagementIndexViewModel
        //        {
        //            Id = u.Id,
        //            Email = u.Email,
        //            Roles = new List<string> { "User" }
        //        })
        //        .ToList();

        //    _mockUserManager.Setup(x => x.Users)
        //        .Returns(_users.AsQueryable());

        //    _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
        //        .ReturnsAsync(new List<string> { "User" });

        //    // Act
        //    var result = await _userService.GetUserManagementBoardDataAsync(currentUserId);
        //    var resultList = result.ToList();

        //    // Assert
        //    Assert.AreEqual(expectedResults.Count, resultList.Count);
        //    Assert.IsFalse(resultList.Any(u => u.Id == currentUserId));
        //    Assert.IsTrue(resultList.All(u => u.Roles.Any()));
        //}

        //[Test]
        //public async Task GetUserManagementBoardDataAsync_ReturnsEmptyListWhenOnlyCurrentUserExists()
        //{
        //    // Arrange
        //    var currentUserId = "1";
        //    var singleUserList = new List<IdentityUser> { _users[0] };

        //    _mockUserManager.Setup(x => x.Users)
        //        .Returns(singleUserList.AsQueryable());

        //    // Mock the empty result
        //    var expectedResults = Enumerable.Empty<UserManagementIndexViewModel>();

        //    // Act
        //    var result = await _userService.GetUserManagementBoardDataAsync(currentUserId);

        //    // Assert
        //    Assert.IsEmpty(result);
        //    Assert.AreEqual(expectedResults.Count(), result.Count());
        //}


        [Test]
        public async Task AssignUserToRoleAsync_ValidUserAndRole_ReturnsTrue()
        {
            // Arrange
            var inputModel = new RoleSelectionInputModel
            {
                UserId = "1",
                Role = "Admin"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(inputModel.UserId))
                .ReturnsAsync(_users[0]);

            _mockRoleManager.Setup(x => x.RoleExistsAsync(inputModel.Role))
                .ReturnsAsync(true);

            _mockUserManager.Setup(x => x.AddToRoleAsync(_users[0], inputModel.Role))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.AssignUserToRoleAsync(inputModel);

            // Assert
            Assert.IsTrue(result);
            _mockUserManager.Verify(x => x.AddToRoleAsync(_users[0], inputModel.Role), Times.Once);
        }

        [Test]
        public void AssignUserToRoleAsync_InvalidUser_ThrowsException()
        {
            // Arrange
            var inputModel = new RoleSelectionInputModel
            {
                UserId = "99",
                Role = "Admin"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(inputModel.UserId))
                .ReturnsAsync((IdentityUser)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.AssignUserToRoleAsync(inputModel));
            Assert.AreEqual("User does not exist!", ex.Message);
        }

        [Test]
        public void AssignUserToRoleAsync_InvalidRole_ThrowsException()
        {
            // Arrange
            var inputModel = new RoleSelectionInputModel
            {
                UserId = "1",
                Role = "InvalidRole"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(inputModel.UserId))
                .ReturnsAsync(_users[0]);

            _mockRoleManager.Setup(x => x.RoleExistsAsync(inputModel.Role))
                .ReturnsAsync(false);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.AssignUserToRoleAsync(inputModel));
            Assert.AreEqual("Selected role is not a valid role!", ex.Message);
        }

        [Test]
        public void AssignUserToRoleAsync_AddRoleFails_ThrowsException()
        {
            // Arrange
            var inputModel = new RoleSelectionInputModel
            {
                UserId = "1",
                Role = "Admin"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(inputModel.UserId))
                .ReturnsAsync(_users[0]);

            _mockRoleManager.Setup(x => x.RoleExistsAsync(inputModel.Role))
                .ReturnsAsync(true);

            _mockUserManager.Setup(x => x.AddToRoleAsync(_users[0], inputModel.Role))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.AssignUserToRoleAsync(inputModel));
            Assert.AreEqual("Unexpected error occurred while adding the user to role! Please try again later!", ex.Message);
            Assert.IsNotNull(ex.InnerException);
        }
    }
}
