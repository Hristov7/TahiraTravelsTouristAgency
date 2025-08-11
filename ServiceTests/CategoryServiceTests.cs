using Core;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Models;
using Moq;
using System.Linq.Expressions;
using ViewModels.ViewModels;

namespace ServiceTests
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<TahiraTravelsDbContext> _mockContext;
        private Mock<DbSet<Category>> _mockCategoriesSet;
        private CategoryService _categoryService;
        private List<Category> _categories;

        [SetUp]
        public void Setup()
        {
            _categories = new List<Category>
            {
                new Category { Id = 1, Name = "Adventure" },
                new Category { Id = 2, Name = "Beach" },
                new Category { Id = 3, Name = "Cultural" }
            };

            // Mock DbSet with async support
            _mockCategoriesSet = new Mock<DbSet<Category>>();

            var queryable = _categories.AsQueryable();

            // Setup sync queryable
            _mockCategoriesSet.As<IQueryable<Category>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Category>(queryable.Provider));
            _mockCategoriesSet.As<IQueryable<Category>>().Setup(m => m.Expression)
                .Returns(queryable.Expression);
            _mockCategoriesSet.As<IQueryable<Category>>().Setup(m => m.ElementType)
                .Returns(queryable.ElementType);
            _mockCategoriesSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator())
                .Returns(() => queryable.GetEnumerator());

            // Setup async enumerable
            _mockCategoriesSet.As<IAsyncEnumerable<Category>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Category>(queryable.GetEnumerator()));

            // Mock DbContext
            _mockContext = new Mock<TahiraTravelsDbContext>();
            _mockContext.Setup(c => c.Categories).Returns(_mockCategoriesSet.Object);

            _categoryService = new CategoryService(_mockContext.Object);
        }

        [Test]
        public void PassAlways()
        {
            Assert.Pass();
        }

        [Test]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            // Arrange
            var expectedCount = 3;
            var expectedFirstCategoryName = "Adventure";

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();
            var resultList = result.ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, resultList.Count);
            Assert.AreEqual(expectedFirstCategoryName, resultList.First().Name);
            Assert.IsInstanceOf<IEnumerable<CategoryViewModel>>(result);
        }

        [Test]
        public async Task GetAllCategoriesAsync_ReturnsCorrectViewModel()
        {
            // Arrange
            var expectedIds = new List<int> { 1, 2, 3 };

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();
            var resultList = result.ToList();

            // Assert
            Assert.AreEqual(expectedIds.Count, resultList.Count);
            CollectionAssert.AreEquivalent(expectedIds, resultList.Select(c => c.Id));
            Assert.IsTrue(resultList.All(c => c.GetType() == typeof(CategoryViewModel)));
        }

        [Test]
        public async Task GetAllCategoriesAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            _categories.Clear();
            _mockCategoriesSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator())
                .Returns(() => _categories.GetEnumerator());

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        // Helper classes for async testing (same as in previous tests)
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
}
