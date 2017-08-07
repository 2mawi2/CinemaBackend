using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Reservation.Domain
{
    public interface IReservationService : IService
    {
        Task<bool> AddAsync(ReservationItem item);
        Task<bool> RemoveAsync(ItemId reservationId);
        Task<ReservationItem> GetByIdAsync(ItemId id);
        Task<IEnumerable<ReservationItem>> GetAllReservationsAsync();
        Task<IEnumerable<ReservationItem>> GetByMovieAsync(string movieName);
        Task<IEnumerable<ReservationItem>> GetByTimeAsync(TimeRange timeRange);

        /// <summary>
        /// This Method deletes the reservation in its own Repository
        /// and also removes the entry from the corresponding Show
        /// </summary>
        Task<bool> RemoveAllWithShowIdAsync(ItemId showId);
    }
}
