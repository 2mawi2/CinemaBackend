using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;
using ReservationItem.UnitTests;
using Show.Domain;
using Show.Service.Service;

namespace Show.UnitTests
{
    [TestClass()]
    public class ShowServiceTests
    {
        [TestMethod]
        public async Task AddShowTest()
        {
            var stateManager = new MockReliableStateManager();
            var ShowService = new ShowService(TestUtils.StatefulServiceContext, stateManager);
            var item = new ShowItem();

            var result = await ShowService.GetByIdAsync(item.Id);
            Assert.AreEqual(null, result);

            var addResult = await ShowService.AddAsync(item);
            Assert.IsTrue(addResult);

            result = await ShowService.GetByIdAsync(item.Id);
            Assert.AreEqual(item, result);
        }

        [TestMethod]
        public async Task RemoveShowTest()
        {
            var showService = ShowService();

            var item = new ShowItem();
            await showService.AddAsync(item);

            var result = await showService.GetByIdAsync(item.Id);
            Assert.AreEqual(item, result);

            await showService.RemoveAsync(item.Id);

            result = await showService.GetByIdAsync(item.Id);
            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public async Task GetShowTest()
        {
            var showService = ShowService();

            var item = new ShowItem();
            await showService.AddAsync(item);

            var result = await showService.GetByIdAsync(item.Id);
            Assert.AreEqual(item, result);
        }


        [TestMethod]
        public async Task TryRemoveReservationFromShowAsync()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);

            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));
            await showService.AddAsync(new ShowItem
            {
                Id = showId,
                MaxReservations = 50,
                Movie = "Bucks Bunny",
                ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            });
            var reservation = new Reservation.Domain.ReservationItem
            {
                ReservationDateTime = new DateTime(2010, 10, 10, 9, 10, 10),
                ShowItemId = showId
            };
            await showService.TryAddReservationToShowAsync(reservation);

            var result = await showService.TryRemoveReservationFromShowAsync(reservation);

            Assert.IsTrue(result);
            Assert.IsTrue(!(await showService.GetByIdAsync(showId)).Reservations.Any());
        }


        [TestMethod]
        public async Task TryAddReservationToShowTest()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);

            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));

            await showService.AddAsync(new ShowItem
            {
                Id = showId,
                MaxReservations = 50,
                Movie = "Bucks Bunny",
                ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            });

            var reservation = new Reservation.Domain.ReservationItem
            {
                ReservationDateTime = new DateTime(2010, 10, 10, 9, 10, 10),
                ShowItemId = showId
            };
            var success = await showService.TryAddReservationToShowAsync(reservation);


            Assert.IsTrue(success);
            var added = (await showService.GetByIdAsync(showId)).Reservations.First();
            Assert.AreEqual(added.Key, reservation.Id);
        }

        [TestMethod]
        public async Task TryAddReservationToShowTest_ShowBookedOut()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);

            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));

            var reservations = new ConcurrentDictionary<ItemId, Reservation.Domain.ReservationItem>();

            var id1 = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822B3"));
            var id2 = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822B2"));
            var id3 = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822B4"));

            reservations.TryAdd(id1, new Reservation.Domain.ReservationItem {Id = id1});
            reservations.TryAdd(id2, new Reservation.Domain.ReservationItem { Id = id2 });
            reservations.TryAdd(id3, new Reservation.Domain.ReservationItem { Id = id3 });

            await showService.AddAsync(new ShowItem
            {
                Id = showId,
                MaxReservations = 3,
                Movie = "Bucks Bunny",
                ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10),
                Reservations = reservations
            });

            var reservation = new Reservation.Domain.ReservationItem
            {
                ReservationDateTime = new DateTime(2010, 10, 10, 9, 10, 10),
                ShowItemId = showId
            };
            var success = await showService.TryAddReservationToShowAsync(reservation);
            Assert.IsFalse(success);
        }

        [TestMethod]
        public async Task TryAddReservationToShowTest_InvalidShowId()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);

            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));

            await showService.AddAsync(new ShowItem
            {
                Id = showId,
                MaxReservations = 50,
                Movie = "Bucks Bunny",
                ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            });

            var invalidID = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C811A8"));
            var reservation = new Reservation.Domain.ReservationItem
            {
                ReservationDateTime = new DateTime(2011, 10, 10, 9, 10, 10),
                ShowItemId = invalidID
            };
            var success = await showService.TryAddReservationToShowAsync(reservation);
            Assert.IsFalse(success);
        }

        [TestMethod]
        public async Task TryAddReservationToShowTest_ReservationIsPastShow()
        {
            //ARRANGE
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);
            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));
            await showService.AddAsync(new ShowItem
            {
                Id = showId,
                MaxReservations = 50,
                Movie = "Bucks Bunny",
                ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            });
            var reservation = new Reservation.Domain.ReservationItem
            {
                ReservationDateTime = new DateTime(2011, 10, 10, 9, 10, 10),
                ShowItemId = showId
            };

            //ACT
            var success = await showService.TryAddReservationToShowAsync(reservation);

            //ASSERT
            Assert.IsFalse(success);
        }

        [TestMethod]
        public async Task GetShowsByMovieTest()
        {
            var showService = ShowService();

            var name = "movieName";
            var showitem = new ShowItem
            {
                Id = new ItemId(),
                MaxReservations = 50,
                Movie = "movieName",
                ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };

            var showitem2 = new ShowItem
            {
                Id = new ItemId(),
                MaxReservations = 50,
                Movie = "movieName",
                ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };

            var showitem3 = new ShowItem
            {
                Id = new ItemId(),
                MaxReservations = 50,
                Movie = "Bucks Bunny",
                ShowDateTime = new DateTime(2010, 10, 10, 10, 10, 10)
            };
            await showService.AddAsync(showitem);
            await showService.AddAsync(showitem2);
            await showService.AddAsync(showitem3);

            var result = await showService.GetShowsByMovieAsync(name);
            result.ForEach(i => Assert.IsTrue(i.Movie.Equals(name)));
            ;
        }

        private static ShowService ShowService()
        {
            var stateManager = new MockReliableStateManager();
            return new ShowService(TestUtils.StatefulServiceContext, stateManager);
        }
    }
}