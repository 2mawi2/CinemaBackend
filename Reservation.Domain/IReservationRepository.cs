using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Model;

namespace Reservation.Domain
{
    public interface IReservationRepository : IRepository<Domain.ReservationItem>
    {
        Task<IEnumerable<ReservationItem>> GetByTimeAsync(TimeRange timeRange);
        Task<IEnumerable<ReservationItem>> GetByMovieAsync(string movieName);
    }
}