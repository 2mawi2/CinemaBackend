using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Factorys;
using Common.Model;
using Common.Utils;
using Reservation.Domain;
using Show.Domain;

namespace Web.Service.Controllers
{
    public class ShowController : ApiController
    {
        private readonly IServiceFactory<IShowService> _showServiceFactory;
        private readonly IServiceFactory<IReservationService> _reservationServiceFactory;
        private readonly IPartitionManager _partitionManager;

        public ShowController(IServiceFactory<IShowService> showServiceFactory, IPartitionManager partitionManager,
            IServiceFactory<IReservationService> reservationServiceFactory)
        {
            _showServiceFactory = showServiceFactory;
            _partitionManager = partitionManager;
            _reservationServiceFactory = reservationServiceFactory;
        }

        [HttpPost]
        [Route("api/shows/by/movie/{movieName}")]
        public async Task<IEnumerable<ShowItem>> GetShowsByMovie(string movieName)
        {
            var shows = await PartitionUtils.SelectManyService(_showServiceFactory, _partitionManager,
                I => I.GetShowsByMovieAsync(movieName).GetAwaiter().GetResult());
            return shows;
        }

        [HttpGet]
        [Route("api/shows/make")]
        public async Task<IHttpActionResult> MakeShow()
        {
            var item = new ShowItem
            {
                MaxReservations = 9,
                Movie = "testMovie",
                ShowDateTime = new DateTime(2017, 12, 12),
            };
            var showService = _showServiceFactory.Get(item.Id.GetPartitionKey());
            if (await showService.AddAsync(item))
            {
                return Ok(item.Id.ToString());
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("api/shows/create)")]
        public async Task<ItemId> CreateShow()
        {
            var item = new ShowItem
            {
                MaxReservations = 9,
                Movie = "testMovie",
                ShowDateTime = new DateTime(2017, 12, 12),
            };
            var showService = _showServiceFactory.Get(item.Id.GetPartitionKey());
            if (await showService.AddAsync(item))
            {
                return item.Id;
            }
            return null;
        }

        [HttpPost]
        [Route("api/shows/post")]
        public async Task<bool> PostShow(ShowItem showItem)
        {
            var showService = _showServiceFactory.Get(showItem.Id.GetPartitionKey());
            return await showService.AddAsync(showItem);
        }

        [HttpPost]
        [Route("api/shows/by/id/{showId}")]
        public async Task<ShowItem> GetById(string showId)
        {
            var showItemId = new ItemId(new Guid(showId));
            var service = _showServiceFactory.Get(showItemId.GetPartitionKey());
            return await service.GetByIdAsync(showItemId);
        }

        [HttpGet]
        [Route("api/shows/all")]
        public async Task<IEnumerable<ShowItem>> GetAll()
        {
            return await PartitionUtils.SelectManyService(_showServiceFactory, _partitionManager,
                I => I.GetAllShowsAsync().GetAwaiter().GetResult());
        }
        
        /// <summary>
        /// Result from RemoveAllWIthShowIdAsync on all services is necessary 
        /// to make client wait on result in async state.  
        /// </summary>
        [HttpPost]
        [Route("api/shows/delete/{showId}")]
        public async Task<bool> DeleteShow(string showId)
        {
            var showItemId = new ItemId(new Guid(showId));
            var service = _showServiceFactory.Get(showItemId.GetPartitionKey());
            var result = await service.RemoveAsync(showItemId);
            var reservationRemoveResult = await PartitionUtils.SelectManyService(_reservationServiceFactory,
                _partitionManager, i => i.RemoveAllWithShowIdAsync(showItemId).GetAwaiter().GetResult());
            return result && reservationRemoveResult.Any(i => i.Equals(true));
        }
    }
}