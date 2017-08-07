using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocks;

namespace Reservation.Service.Repositorys.Tests
{
    [TestClass()]
    public class ReservationRepositoryTests
    {
        [TestMethod()]
        public void ReservationRepositoryTest()
        {
            var stateManager = new MockReliableStateManager();
            var repository = new ReservationRepository(stateManager);
            Assert.IsNotNull(repository);
            Assert.AreEqual("reservationItems", ReservationRepository.ReservationItemDictId);
        }
    }
}