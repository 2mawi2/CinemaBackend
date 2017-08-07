using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Factorys;
using Common.Model;
using Common.Utils;
using Reservation.Domain;

namespace Web.Service.Controllers
{
    public class ReservationController : ApiController
    {
        private readonly IServiceFactory<IReservationService> _serviceFactory;
        private readonly IPartitionManager _partitionManager;

        public ReservationController(IServiceFactory<IReservationService> serviceFactory,
            IPartitionManager partitionManager)
        {
            _serviceFactory = serviceFactory;
            _partitionManager = partitionManager;
        }

        [HttpPost]
        [Route("api/reservations/by/movie/{movieName}")]
        public async Task<IEnumerable<ReservationItem>> GetReservationsByMovie(string movieName)
        {
            var reservations = await PartitionUtils.SelectManyService(_serviceFactory, _partitionManager,
                I => I.GetByMovieAsync(movieName).GetAwaiter().GetResult());
            return reservations;
        }

        [HttpPost]
        [Route("api/reservations/post/{reservationItem}")]
        public async Task<bool> PostReservation(ReservationItem reservationItem)
        {
            if (reservationItem.Id == null)
            {
                return false;
            }
            return await ReservationService(reservationItem.Id).AddAsync(reservationItem);
        }

        private IReservationService ReservationService(ItemId reservationItemId)
        {
            return _serviceFactory.Get(reservationItemId.GetPartitionKey());
        }

        [HttpPost]
        [Route("api/reservations/create/{showId}")]
        public async Task<IHttpActionResult> CreateReservation(string showId)
        {
            var reservation = new ReservationItem
            {
                ShowItemId = new ItemId(new Guid(showId))
            };
            var id = reservation.Id;
            if (await ReservationService(id).AddAsync(reservation))
                return Ok(id);
            return BadRequest();
        }

        [HttpPost]
        [Route("api/reservations/by/id/{reservationId}")]
        public async Task<ReservationItem> GetById(string reservationId)
        {
            var reservationItemId = new ItemId(new Guid(reservationId));
            return await ReservationService(reservationItemId).GetByIdAsync(reservationItemId);
        }

        [HttpGet]
        [Route("api/reservations/all")]
        public async Task<IEnumerable<ReservationItem>> GetAll()
        {
            return await PartitionUtils.SelectManyService(_serviceFactory, _partitionManager,
                I => I.GetAllReservationsAsync().GetAwaiter().GetResult());
        }

        [HttpPost]
        [Route("api/reservations/by/time/{timeRange}")]
        public async Task<IEnumerable<ReservationItem>> GetReservationByTime(TimeRange timeRange)
        {
            return await PartitionUtils.SelectManyService(_serviceFactory, _partitionManager,
                I => I.GetByTimeAsync(timeRange).GetAwaiter().GetResult());
        }

        [HttpPost]
        [Route("api/reservations/delete/{reservationId}")]
        public async Task<IHttpActionResult> DeleteReservation(string reservationId)
        {
            var id = new ItemId(new Guid(reservationId));
            await ReservationService(id).RemoveAsync(id);
            return Ok(true);
        }
    }
}