using Core;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Models;
using Moq;
using System.Linq.Expressions;
using ViewModels.ViewModels;

namespace ServiceTests
{
    [TestFixture]
    public class TourServiceTests
    {
        private Mock<TahiraTravelsDbContext> _mockContext;
        private TourService _tourService;
        private Mock<DbSet<Destination>> _mockDestinationsSet;
        private Mock<DbSet<Category>> _mockCategoriesSet;
        private Mock<DbSet<UserDestination>> _mockUserDestinationsSet;
        private List<Destination> _destinations;
        private List<Category> _categories;
        private List<UserDestination> _userDestinations;

        [SetUp]
        public void Setup()
        {
            // Initialize test data
            _categories = new List<Category>
            {
                new Category { Id = 1, Name = "Adventure" },
                new Category { Id = 2, Name = "Beach" }
            };

            var author = new IdentityUser { Id = "author1", UserName = "author1@example.com" };
            var otherUser = new IdentityUser { Id = "user1", UserName = "user1@example.com" };

            _destinations = new List<Destination>
            {
                new Destination {
                    Id = 1,
                    Name = "Mountain Trek",
                    Description = "Amazing mountain views",
                    ImageUrl = "image1.jpg",
                    CategoryId = 1,
                    Category = _categories[0],
                    AuthorId = "author1",
                    Author = author,
                    CreatedOn = DateTime.Now.AddDays(-10),
                    IsDeleted = false,
                    UsersDestinations = new List<UserDestination>()
                },
                new Destination {
                    Id = 2,
                    Name = "Beach Vacation",
                    Description = "Relaxing beach holiday",
                    ImageUrl = "image2.jpg",
                    CategoryId = 2,
                    Category = _categories[1],
                    AuthorId = "author1",
                    Author = author,
                    CreatedOn = DateTime.Now.AddDays(-5),
                    IsDeleted = false,
                    UsersDestinations = new List<UserDestination> { new UserDestination { UserId = "user1", DestinationId = 2 } }
                },
                new Destination {
                    Id = 3,
                    Name = "Deleted Tour",
                    Description = "This tour is deleted",
                    ImageUrl = "image3.jpg",
                    CategoryId = 1,
                    Category = _categories[0],
                    AuthorId = "author1",
                    Author = author,
                    CreatedOn = DateTime.Now.AddDays(-3),
                    IsDeleted = true,
                    UsersDestinations = new List<UserDestination>()
                }
            };

            _userDestinations = new List<UserDestination>
            {
                new UserDestination { UserId = "user1", DestinationId = 2 }
            };

            // Mock DbSets with async support
            _mockDestinationsSet = CreateMockDbSet(_destinations.AsQueryable());
            _mockCategoriesSet = CreateMockDbSet(_categories.AsQueryable());
            _mockUserDestinationsSet = CreateMockDbSet(_userDestinations.AsQueryable());

            // Mock DbContext
            _mockContext = new Mock<TahiraTravelsDbContext>();
            _mockContext.Setup(c => c.Destinations).Returns(_mockDestinationsSet.Object);
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoriesSet.Object);
            _mockContext.Setup(c => c.UsersDestinations).Returns(_mockUserDestinationsSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _tourService = new TourService(_mockContext.Object);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new CustomAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new CustomAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Expression)
                .Returns(data.Expression);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.ElementType)
                .Returns(data.ElementType);
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => data.GetEnumerator());

            return mockSet;
        }
        [Test]
        public void PassAlways()
        {
            Assert.Pass();
        }

        [Test]
        public async Task GetAllToursAsync_ReturnsNonDeletedTours()
        {
            // Arrange
            var userId = "user1";

            // Act
            var result = await _tourService.GetAllToursAsync(userId);
            var resultList = result.ToList();

            // Assert
            Assert.AreEqual(2, resultList.Count);
            Assert.IsTrue(resultList.All(t => t.Name != "Deleted Tour"));
            Assert.IsTrue(resultList.Any(t => t.IsSaved));
        }

        [Test]
        public async Task GetTourDetailsAsync_ValidId_ReturnsTourDetails()
        {
            // Arrange
            var tourId = 1;
            var userId = "user1";

            // Act
            var result = await _tourService.GetTourDetailsAsync(tourId, userId);

            // Assert
            Assert.AreEqual("Mountain Trek", result.Name);
            Assert.AreEqual("Adventure", result.Category);
            Assert.IsFalse(result.IsSaved);
            Assert.IsFalse(result.IsAuthor);
        }

        [Test]
        public void GetTourDetailsAsync_InvalidId_ThrowsException()
        {
            // Arrange
            var invalidId = 99;
            var userId = "user1";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _tourService.GetTourDetailsAsync(invalidId, userId));
        }

        [Test]
        public async Task CreateTourAsync_ValidModel_CreatesNewTour()
        {
            // Arrange
            var model = new TourCreateInputModel
            {
                Name = "New Tour",
                Description = "New Description",
                ImageUrl = "new.jpg",
                CategoryId = 1,
                CreatedOn = DateTime.Now.ToString("yyyy-MM-dd")
            };
            var userId = "author1";

            // Act
            await _tourService.CreateTourAsync(model, userId);

            // Assert
            _mockDestinationsSet.Verify(m => m.AddAsync(It.IsAny<Destination>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetTourForEditAsync_ValidTour_ReturnsEditModel()
        {
            // Arrange
            var tourId = 1;
            var userId = "author1";

            // Act
            var result = await _tourService.GetTourForEditAsync(tourId, userId);

            // Assert
            Assert.AreEqual("Mountain Trek", result.Name);
            Assert.AreEqual(2, result.Categories.Count());
        }

        [Test]
        public void GetTourForEditAsync_InvalidTour_ThrowsException()
        {
            // Arrange
            var invalidId = 99;
            var userId = "author1";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _tourService.GetTourForEditAsync(invalidId, userId));
        }

        [Test]
        public async Task EditTourAsync_ValidTour_UpdatesTour()
        {
            // Arrange
            var model = new TourEditInputModel
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description",
                ImageUrl = "updated.jpg",
                CategoryId = 2,
                CreatedOn = DateTime.Now.ToString("yyyy-MM-dd")
            };
            var userId = "author1";

            // Act
            await _tourService.EditTourAsync(model, userId);

            // Assert
            var tour = _destinations.First(d => d.Id == 1);
            Assert.AreEqual("Updated Name", tour.Name);
            Assert.AreEqual(2, tour.CategoryId);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void EditTourAsync_InvalidTour_ThrowsException()
        {
            // Arrange
            var model = new TourEditInputModel
            {
                Id = 99,
                Name = "Invalid Tour",
                Description = "Invalid Description",
                ImageUrl = "invalid.jpg",
                CategoryId = 1,
                CreatedOn = DateTime.Now.ToString("yyyy-MM-dd")
            };
            var userId = "author1";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _tourService.EditTourAsync(model, userId));
        }

        [Test]
        public async Task GetTourForDeleteAsync_ValidTour_ReturnsDeleteModel()
        {
            // Arrange
            var tourId = 1;
            var userId = "author1";

            // Act
            var result = await _tourService.GetTourForDeleteAsync(tourId, userId);

            // Assert
            Assert.AreEqual("Mountain Trek", result.Name);
            Assert.AreEqual("author1@example.com", result.Author);
        }

        [Test]
        public void GetTourForDeleteAsync_InvalidTour_ThrowsException()
        {
            // Arrange
            var invalidId = 99;
            var userId = "author1";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _tourService.GetTourForDeleteAsync(invalidId, userId));
        }

        [Test]
        public async Task DeleteTourAsync_ValidTour_SetsIsDeleted()
        {
            // Arrange
            var tourId = 1;
            var userId = "author1";

            // Act
            await _tourService.DeleteTourAsync(tourId, userId);

            // Assert
            var tour = _destinations.First(d => d.Id == 1);
            Assert.IsTrue(tour.IsDeleted);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void DeleteTourAsync_InvalidTour_ThrowsException()
        {
            // Arrange
            var invalidId = 99;
            var userId = "author1";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _tourService.DeleteTourAsync(invalidId, userId));
        }

        [Test]
        public async Task SaveTourAsync_NotAlreadySaved_AddsToFavorites()
        {
            // Arrange
            var tourId = 1;
            var userId = "user1";

            // Act
            await _tourService.SaveTourAsync(tourId, userId);

            // Assert
            _mockUserDestinationsSet.Verify(m => m.AddAsync(It.IsAny<UserDestination>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveTourAsync_AlreadySaved_DoesNothing()
        {
            // Arrange
            var tourId = 2;
            var userId = "user1";

            // Act
            await _tourService.SaveTourAsync(tourId, userId);

            // Assert
            _mockUserDestinationsSet.Verify(m => m.AddAsync(It.IsAny<UserDestination>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task RemoveTourFromFavoritesAsync_Exists_RemovesFromFavorites()
        {
            // Arrange
            var tourId = 2;
            var userId = "user1";

            // Act
            await _tourService.RemoveTourFromFavoritesAsync(tourId, userId);

            // Assert
            _mockUserDestinationsSet.Verify(m => m.Remove(It.IsAny<UserDestination>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RemoveTourFromFavoritesAsync_NotExists_DoesNothing()
        {
            // Arrange
            var tourId = 1;
            var userId = "user1";

            // Act
            await _tourService.RemoveTourFromFavoritesAsync(tourId, userId);

            // Assert
            _mockUserDestinationsSet.Verify(m => m.Remove(It.IsAny<UserDestination>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetFavoriteToursAsync_ReturnsUserFavorites()
        {
            // Arrange
            var userId = "user1";

            // Clear existing user destinations and set up proper relationships
            _userDestinations.Clear();

            var favoriteTour = _destinations[1]; // Beach Vacation
            favoriteTour.Category = _categories[1]; // Beach
            favoriteTour.UsersDestinations = new List<UserDestination>();

            var userDestination = new UserDestination
            {
                UserId = userId,
                DestinationId = favoriteTour.Id,
                Destination = favoriteTour,  // Set the navigation property
                User = new IdentityUser { Id = userId } // Set user if needed
            };

            favoriteTour.UsersDestinations.Add(userDestination);
            _userDestinations.Add(userDestination);

            // Recreate the mock DbSet with updated data
            _mockUserDestinationsSet = CreateMockDbSet(_userDestinations.AsQueryable());
            _mockContext.Setup(c => c.UsersDestinations).Returns(_mockUserDestinationsSet.Object);

            // Act
            var result = await _tourService.GetFavoriteToursAsync(userId);
            var resultList = result.ToList();

            // Assert
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("Beach Vacation", resultList[0].Name);
            Assert.AreEqual("Beach", resultList[0].Category);
            Assert.AreEqual("image2.jpg", resultList[0].ImageUrl);
        }
    }

    // Renamed helper classes to avoid conflicts
    internal class CustomAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _underlyingProvider;

        public CustomAsyncQueryProvider(IQueryProvider underlyingProvider)
        {
            _underlyingProvider = underlyingProvider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new CustomAsyncEnumerable<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new CustomAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _underlyingProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _underlyingProvider.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(Expression) })
                .MakeGenericMethod(expectedResultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class CustomAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public CustomAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public CustomAsyncEnumerable(Expression expression) : base(expression) { }

        IQueryProvider IQueryable.Provider => new CustomAsyncQueryProvider<T>(this);

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new CustomAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }

    internal class CustomAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _source;

        public CustomAsyncEnumerator(IEnumerator<T> source)
        {
            _source = source;
        }

        public T Current => _source.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_source.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _source.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
