using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model;
using Microsoft.ServiceFabric.Services.Remoting;
using Reservation.Domain;

namespace Show.Domain
{
    public interface IShowService : IService
    {
        Task<bool> AddAsync(ShowItem item);
        Task<bool> RemoveAsync(ItemId id);
        Task<ShowItem> GetByIdAsync(ItemId id);
        Task<IEnumerable<ShowItem>> GetAllShowsAsync();
        Task<bool> TryAddReservationToShowAsync(ReservationItem reservation);
        Task<IEnumerable<ShowItem>> GetShowsByMovieAsync(string movieName);
        Task<bool> TryRemoveReservationFromShowAsync(ReservationItem item);
    }
}
