using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Data;
using Common.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;

namespace Common.UnitTests.Data
{
    [TestClass]
    public class RepositoryTests
    {
        private class TestRepository : Repository<Foo>
        {
            public TestRepository(IReliableStateManager stateManager, string reservationItemDictId) : base(stateManager,
                reservationItemDictId)
            {
            }
        }

        private class Foo : IItem
        {
            public ItemId Id { get; set; } = new ItemId();
            public string Attribute { get; set; }
        }

        private IReliableStateManager _stateManager;
        private TestRepository _testRepository;
        private const string DictId = "someId";


        [TestInitialize]
        public void SetUp()
        {
            _stateManager = new MockReliableStateManager();
            _testRepository = new TestRepository(_stateManager, DictId);
        }


        [TestMethod]
        public async Task FindTest()
        {
            var foo = new Foo();
            var foo2 = new Foo();
            var foo3 = new Foo();

            await _testRepository.AddAsync(foo);
            await _testRepository.AddAsync(foo2);
            await _testRepository.AddAsync(foo3);

            var result = await _testRepository.FindAsync(i => i.Id.Equals(foo2.Id) || i.Id.Equals(foo3.Id));
            CollectionAssert.AreEquivalent(new List<Foo> {foo2, foo3}, result.ToList());
        }

        [TestMethod]
        public async Task AddTest()
        {
            var foo = new Foo
            {
                Id = new ItemId(Guid.NewGuid())
            };
            var result = await _testRepository.AddAsync(foo);
            Assert.IsTrue(result);
            Assert.AreEqual((await _testRepository.GetAllAsync()).First(), foo);
        }

        [TestMethod]
        public async Task AddEnumerableTest()
        {
            var enumerable = new List<Foo>
            {
                new Foo(),
                new Foo(),
                new Foo(),
            };
            var result = await _testRepository.AddAsync(enumerable);
            Assert.IsTrue(result);
            CollectionAssert.AreEquivalent((await _testRepository.GetAllAsync()).ToList(), enumerable);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var first = new Foo();
            await _testRepository.AddAsync(first);
            var newValue = new Foo
            {
                Attribute = "new attribute"
            };
            var result = await _testRepository.UpdateAsync(first.Id, newValue);

            Assert.IsTrue(result);
            Assert.AreEqual((await _testRepository.GetAllAsync()).First(), newValue);
        }

        [TestMethod]
        public async Task RemoveTest()
        {
            var foo = new Foo
            {
                Id = new ItemId(Guid.NewGuid())
            };
            await _testRepository.AddAsync(foo);
            Assert.AreEqual(1, (await _testRepository.GetAllAsync()).Count());
            await _testRepository.RemoveAsync(foo.Id);
            Assert.AreEqual(0, (await _testRepository.GetAllAsync()).Count());
        }

        [TestMethod]
        public async Task GetByIdTest()
        {
            var foo = new Foo
            {
                Id = new ItemId(Guid.NewGuid())
            };
            await _testRepository.AddAsync(foo);
            Assert.AreEqual(1, (await _testRepository.GetAllAsync()).Count());
            var result = await _testRepository.GetByIdAsync(foo.Id);
            Assert.AreEqual(foo, result);
            var result2 = await _testRepository.GetByIdAsync(new Foo().Id);
            Assert.AreEqual(null, result2);
        }

        [TestMethod]
        public async Task GetAllTest()
        {
            await _testRepository.AddAsync(new Foo());
            await _testRepository.AddAsync(new Foo());
            await _testRepository.AddAsync(new Foo());
            Assert.AreEqual(3, (await _testRepository.GetAllAsync()).Count());
        }
    }
}