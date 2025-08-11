using Core;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Models;
using Moq;
using System.Linq.Expressions;

namespace ServiceTests
{
    [TestFixture]
    public class BookingServiceTests
    {
        private Mock<TahiraTravelsDbContext> _mockContext;
        private Mock<DbSet<Booking>> _mockBookingsSet;
        private BookingService _bookingService;
        private List<Booking> _bookings;

        [SetUp]
        public void Setup()
        {
            _bookings = new List<Booking>
            {
                new Booking { Id = 1, UserId = "user1", TourId = 1, NumberOfPeople = 2, BookingDate = DateTime.UtcNow.AddDays(7) },
                new Booking { Id = 2, UserId = "user1", TourId = 2, NumberOfPeople = 1, BookingDate = DateTime.UtcNow.AddDays(14) },
                new Booking { Id = 3, UserId = "user2", TourId = 1, NumberOfPeople = 4, BookingDate = DateTime.UtcNow.AddDays(21) }
            };

            // Mock DbSet with async support
            _mockBookingsSet = new Mock<DbSet<Booking>>();

            // Setup sync queryable
            var queryable = _bookings.AsQueryable();
            _mockBookingsSet.As<IQueryable<Booking>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Booking>(queryable.Provider));
            _mockBookingsSet.As<IQueryable<Booking>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockBookingsSet.As<IQueryable<Booking>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockBookingsSet.As<IQueryable<Booking>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // Setup async enumerable
            _mockBookingsSet.As<IAsyncEnumerable<Booking>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Booking>(queryable.GetEnumerator()));

            // Setup FindAsync
            _mockBookingsSet.Setup(x => x.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((object[] ids) => _bookings.FirstOrDefault(b => b.Id == (int)ids[0]));

            // Setup AddAsync
            _mockBookingsSet.Setup(m => m.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
                .Callback<Booking, CancellationToken>((b, ct) => _bookings.Add(b))
                .ReturnsAsync((Booking b, CancellationToken ct) => null);

            // Mock DbContext
            _mockContext = new Mock<TahiraTravelsDbContext>();
            _mockContext.Setup(c => c.Bookings).Returns(_mockBookingsSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            _bookingService = new BookingService(_mockContext.Object);
        }
        [Test]
        public void PassAlways()
        {
            Assert.Pass();
        }

        [Test]
        public async Task CreateBookingAsync_ValidBooking_AddsBookingToDatabase()
        {
            // Arrange
            var newBooking = new Booking
            {
                TourId = 3,
                NumberOfPeople = 3,
                BookingDate = DateTime.UtcNow.AddDays(10)
            };
            var userId = "user1";

            // Change the setup to expect Add instead of AddAsync
            _mockBookingsSet.Setup(m => m.Add(It.IsAny<Booking>()))
                           .Callback<Booking>(b => _bookings.Add(b))
                           .Returns((EntityEntry<Booking>)null);

            // Act
            await _bookingService.CreateBookingAsync(newBooking, userId);

            // Assert
            _mockBookingsSet.Verify(m => m.Add(It.IsAny<Booking>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.AreEqual(userId, newBooking.UserId);
            Assert.IsTrue(_bookings.Any(b => b.UserId == userId && b.TourId == 3));
        }

        [Test]
        public async Task GetUserBookingsAsync_ValidUserId_ReturnsUserBookings()
        {
            // Arrange
            var userId = "user1";
            var expectedCount = 2;

            // Act
            var result = await _bookingService.GetUserBookingsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.IsTrue(result.All(b => b.UserId == userId));
        }

        [Test]
        public async Task GetUserBookingsAsync_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = "nonexistent-user";

            // Act
            var result = await _bookingService.GetUserBookingsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task DeleteBookingAsync_ValidBookingId_DeletesBookingAndReturnsTrue()
        {
            // Arrange
            var bookingId = 1;
            var userId = "user1";
            var initialCount = _bookings.Count;

            // Setup Remove to actually remove from our in-memory list
            _mockBookingsSet.Setup(m => m.Remove(It.IsAny<Booking>()))
                           .Callback<Booking>(b => _bookings.Remove(b));

            // Act
            var result = await _bookingService.DeleteBookingAsync(bookingId, userId);

            // Assert
            Assert.IsTrue(result);
            _mockBookingsSet.Verify(m => m.Remove(It.Is<Booking>(b => b.Id == bookingId)), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task DeleteBookingAsync_InvalidBookingId_ReturnsFalse()
        {
            // Arrange
            var bookingId = 99; // Doesn't exist
            var userId = "user1";
            var initialCount = _bookings.Count;

            // Act
            var result = await _bookingService.DeleteBookingAsync(bookingId, userId);

            // Assert
            Assert.IsFalse(result);
            _mockBookingsSet.Verify(m => m.Remove(It.IsAny<Booking>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            Assert.AreEqual(initialCount, _bookings.Count);
        }

        [Test]
        public async Task DeleteBookingAsync_ValidIdButWrongUser_ReturnsFalse()
        {
            // Arrange
            var bookingId = 1; // Belongs to user1
            var userId = "user2"; // Different user
            var initialCount = _bookings.Count;

            // Act
            var result = await _bookingService.DeleteBookingAsync(bookingId, userId);

            // Assert
            Assert.IsFalse(result);
            _mockBookingsSet.Verify(m => m.Remove(It.IsAny<Booking>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            Assert.AreEqual(initialCount, _bookings.Count);
        }
    }

    // Helper classes for async testing
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
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

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }
}
