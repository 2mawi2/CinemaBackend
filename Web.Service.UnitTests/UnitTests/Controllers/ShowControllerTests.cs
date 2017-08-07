using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;
using Reservation.UnitTests;
using Show.Domain;

namespace Web.Service.Controllers.Tests
{
    [TestClass()]
    public class ShowControllerTests
    {

        private static ShowController ShowController()
        {
            var factoryMock = new MockShowServiceFactory();
            var partitionManager = new MockPartitionManager();
            var reservationFactoryMock = new MockReservationServiceFactory();
            var controller = new ShowController(factoryMock, partitionManager, reservationFactoryMock);
            return controller;
        }

        private ShowController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _controller = ShowController();
        }

        [TestMethod()]
        public async Task GetShowsByMovieTest()
        {
            var movieName = "Jurrasic Park";
            var result = await _controller.GetShowsByMovie(movieName);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task MakeShowTest()
        {
            var result = await _controller.MakeShow() as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Content);
        }

        [TestMethod()]
        public async Task CreateShowTest()
        {
            var result = await _controller.CreateShow();
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task DeleteShowTest()
        {
            await _controller.DeleteShow(Guid.NewGuid().ToString());
        }

        [TestMethod()]
        public async Task PostShowTest()
        {
            var result = await _controller.PostShow(new ShowItem());
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task GetByIdTest()
        {
            var result = await _controller.GetById(Guid.NewGuid().ToString());
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task GetAllTest()
        {
            var result = await _controller.GetAll();
            Assert.IsNotNull(result);
        }
    }
}