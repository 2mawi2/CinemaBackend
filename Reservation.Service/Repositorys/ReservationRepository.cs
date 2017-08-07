using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Data;
using Common.Model;
using Microsoft.ServiceFabric.Data;
using Reservation.Domain;

namespace Reservation.Service.Repositorys
{
    public sealed class ReservationRepository : Repository<ReservationItem>, IReservationRepository
    {
        public const string ReservationItemDictId = "reservationItems";

        public ReservationRepository(IReliableStateManager stateManager) : base(stateManager, ReservationItemDictId)
        {
        }

        public async Task<IEnumerable<ReservationItem>> GetByTimeAsync(TimeRange timeRange)
        {
            return await FindAsync(i => timeRange.IsBetween(i.ReservationDateTime));
        }

        public async Task<IEnumerable<ReservationItem>> GetByMovieAsync(string movieName)
        {
            return await FindAsync(i => i.Movie.Equals(movieName));
        }
    }
}