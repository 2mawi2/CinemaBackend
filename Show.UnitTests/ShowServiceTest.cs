using System;
using System.Collections.Concurrent;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading.Tasks;
using Common.Model;
using Mocks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ReservationItem.UnitTests;
using Show.Domain;
using Show.Service.Service;

namespace Show.UnitTests
{
    [TestFixture()]
    public class ShowServiceTests
    {
        [Test]
        public async Task AddShowTest()
        {
            var stateManager = new MockReliableStateManager();
            var ShowService = new ShowService(TestUtils.StatefulServiceContext, stateManager);
            var item = new ShowItem();

            var result = await ShowService.GetById(item.Id);
            Assert.AreEqual(null, result);

            var addResult = await ShowService.Add(item);
            Assert.IsTrue(addResult);

            result = await ShowService.GetById(item.Id);
            Assert.AreEqual(item, result);
        }

        [Test]
        public async Task RemoveShowTest()
        {
            var showService = ShowService();

            var item = new ShowItem();
            await showService.Add(item);

            var result = await showService.GetById(item.Id);
            Assert.AreEqual(item, result);

            await showService.Remove(item.Id);

            result = await showService.GetById(item.Id);
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task GetShowTest()
        {
            var showService = ShowService();

            var item = new Domain.ShowItem();
            await showService.Add(item);

            var result = await showService.GetById(item.Id);
            Assert.AreEqual(item, result);
        }

        [Test]
        public async Task TryAddReservationToShowTest()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);

            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));

            await showService.Add(new ShowItem
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
            var success = await showService.TryAddReservationToShow(reservation);


            Assert.IsTrue(success);
            var added = (await showService.GetById(showId)).Reservations.First();
            Assert.AreEqual(added, reservation.Id);
        }

        [Test]
        public async Task TryAddReservationToShowTest_ShowBookedOut()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);

            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));

            var reservations = new BlockingCollection<ItemId>();

            reservations.Add(new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822B3")));
            reservations.Add(new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822B2")));
            reservations.Add(new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822B3")));

            await showService.Add(new ShowItem
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
            var success = await showService.TryAddReservationToShow(reservation);
            Assert.IsFalse(success);
        }

        [Test]
        public async Task TryAddReservationToShowTest_InvalidShowId()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);

            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));

            await showService.Add(new ShowItem
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
            var success = await showService.TryAddReservationToShow(reservation);
            Assert.IsFalse(success);
        }

        [Test]
        public async Task TryAddReservationToShowTest_ShowInThePast()
        {
            var stateManager = new MockReliableStateManager();
            var showService = new ShowService(TestUtils.StatefulServiceContext, stateManager);

            var showId = new ItemId(new Guid("936DA01F-9ABD-4D9D-80C7-02AF85C822A8"));

            await showService.Add(new ShowItem
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
            var success = await showService.TryAddReservationToShow(reservation);
            Assert.IsFalse(success);
        }

        private static ShowService ShowService()
        {
            var stateManager = new MockReliableStateManager();
            return new ShowService(TestUtils.StatefulServiceContext, stateManager);
        }
    }
}