using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Common.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;
using Web.Service.Controllers;

namespace Web.Service.UnitTests.Controllers
{
    [TestClass]
    public class ReservationControllerTests
    {
        private ReservationController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _controller = ReservationController();
        }

        [TestMethod]
        public async Task GetReservationsByTimeTest()
        {
            var movieName = "Jurrasic Park";
            var result = await _controller.GetReservationByTime(new TimeRange(new DateTime(2009, 12, 12, 12, 12, 12), new DateTime(2010, 12, 12, 12, 12, 12)));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetReservationsByMovieTest()
        {
            var movieName = "Jurrasic Park";
            var result = await _controller.GetReservationsByMovie(movieName);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task PostReservationTest()
        {
            var result = await _controller.PostReservation(new Reservation.Domain.ReservationItem());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteReservationTest()
        {
            var result = await _controller.DeleteReservation(Guid.NewGuid().ToString()) as OkNegotiatedContentResult<bool>;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetAllTest()
        {
            var result = await _controller.GetAll();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetByIdTest()
        {
            var result = await _controller.GetById(Guid.NewGuid().ToString());
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task CreateReservationTest()
        {
            var result = await _controller.CreateReservation(Guid.NewGuid().ToString());
            Assert.IsNotNull(result);
        }


        private static ReservationController ReservationController()
        {
            var factoryMock = new MockReservationServiceFactory();
            var partitionManager = new MockPartitionManager();
            var controller = new ReservationController(factoryMock, partitionManager);
            return controller;
        }

        
    }
}