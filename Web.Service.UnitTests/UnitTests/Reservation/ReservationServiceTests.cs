using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;
using Reservation.Service.Service;
using ReservationItem.UnitTests;
using Show.Domain;

namespace Reservation.UnitTests
{
    [TestClass]
    public class ReservationServiceTests
    {
        [TestMethod]
        public async Task AddReservationTest()
        {
            var stateManager = new MockReliableStateManager();
            var testShowDate = new DateTime(2012, 10, 10);
            var showService = new MockShowService
            {
                TryAddReservationToShowFunc = reservationItem => Task.FromResult(true),
                GetByIdFunc = id => Task.FromResult(new ShowItem
                {
                    Id = id,
                    ShowDateTime = testShowDate
                })
            };
            var showServiceFactory = new MockShowServiceFactory(showService);

            var reservationService = new ReservationService(TestUtils.StatefulServiceContext, stateManager,
                showServiceFactory);
            var item = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
            };

            var result = await reservationService.GetByIdAsync(item.Id);
            Assert.AreEqual(null, result);

            var addResult = await reservationService.AddAsync(item);
            Assert.IsTrue(addResult);

            result = await reservationService.GetByIdAsync(item.Id);
            Assert.AreEqual(item, result);
            Assert.IsTrue(item.ReservationDateTime.HasValue);
        }

        [TestMethod]
        public async Task RemoveReservationTest()
        {
            var reservationService = ReservationService();

            var item = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            await reservationService.AddAsync(item);

            var result = await reservationService.GetByIdAsync(item.Id);
            Assert.AreEqual(item, result);

            await reservationService.RemoveAsync(item.Id);

            result = await reservationService.GetByIdAsync(item.Id);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public async Task GetReservationTest()
        {
            var reservationService = ReservationService();

            var item = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            await reservationService.AddAsync(item);

            var result = await reservationService.GetByIdAsync(item.Id);
            Assert.AreEqual(item, result);
        }

        [TestMethod]
        public async Task GetAllAsyncTest()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new MockShowService
            {
                TryAddReservationToShowFunc = reservationItem => Task.FromResult(true)
            };
            var showServiceFactory = new MockShowServiceFactory(showService);
            var reservationService = new ReservationService(TestUtils.StatefulServiceContext, stateManager,
                showServiceFactory);

            var item = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            var item2 = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            var item3 = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            await reservationService.AddAsync(item);
            await reservationService.AddAsync(item2);
            await reservationService.AddAsync(item3);


            var result = await reservationService.GetAllReservationsAsync();
            Assert.IsTrue(result.ToList().Contains(item));
            Assert.IsTrue(result.ToList().Contains(item2));
            Assert.IsTrue(result.ToList().Contains(item3));
        }


        [TestMethod]
        public async Task GetByTimeAsync()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new MockShowService
            {
                TryAddReservationToShowFunc = reservationItem => Task.FromResult(true),
                GetByIdFunc = id => Task.FromResult(new ShowItem{Id = id, ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10) })
            };
            var showServiceFactory = new MockShowServiceFactory(showService);
            var reservationService = new ReservationService(TestUtils.StatefulServiceContext, stateManager,
                showServiceFactory);
            var testMovieName = "testMovie";
            var item = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                Movie = testMovieName,
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            var item2 = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                Movie = testMovieName,
                ReservationDateTime = new DateTime(2010, 10, 11, 10, 10, 10)
            };
            var item3 = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                Movie = "anotherMovie",
                ReservationDateTime = new DateTime(2010, 10, 3, 10, 10, 10)
            };
            await reservationService.AddAsync(item);
            await reservationService.AddAsync(item2);
            await reservationService.AddAsync(item3);


            var result = await reservationService.GetByTimeAsync(new TimeRange(
                new DateTime(2010, 10, 9, 10, 10, 10),
                new DateTime(2010, 10, 12, 10, 10, 10)));
            Assert.IsTrue(result.Contains(item));
            Assert.IsTrue(result.Contains(item2));
            Assert.IsFalse(result.Contains(item3));
        }


        [TestMethod]
        public async Task GetByMovieAsyncTest()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new MockShowService
            {
                TryAddReservationToShowFunc = reservationItem => Task.FromResult(true)
            };
            var showServiceFactory = new MockShowServiceFactory(showService);
            var reservationService = new ReservationService(TestUtils.StatefulServiceContext, stateManager,
                showServiceFactory);
            var testMovieName = "testMovie";
            var item = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                Movie = testMovieName,
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            var item2 = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                Movie = testMovieName,
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            var item3 = new Domain.ReservationItem
            {
                ShowItemId = new ItemId(Guid.NewGuid()),
                Movie = "anotherMovie",
                ReservationDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            await reservationService.AddAsync(item);
            await reservationService.AddAsync(item2);
            await reservationService.AddAsync(item3);


            var result = await reservationService.GetByMovieAsync(testMovieName);
            result.ForEach(i => Assert.IsTrue(i.Movie.Equals(testMovieName)));
        }


        private static ReservationService ReservationService()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new MockShowServiceFactory();
            return new ReservationService(TestUtils.StatefulServiceContext, stateManager, showService);
        }
    }
}