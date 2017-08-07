using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model;
using Reservation.Domain;

namespace Mocks
{
    public class MockReservationService : IReservationService
    {
        public MockReservationService()
        {
            AddReservationFunc = reservation => Task.FromResult(true);
            RemoveReservationFunc = id => Task.FromResult(true);
            RemoveAllWithShowIdAsyncFunc = id => Task.FromResult(true);
            GetReservationFunc = id => Task.FromResult(new ReservationItem());
            GetAllReservationsFunc = () => Task.FromResult((IEnumerable<ReservationItem>)new List<ReservationItem>());
            GetByMovieFunc = id => Task.FromResult((IEnumerable<ReservationItem>)new List<ReservationItem>());
            GetByTimeFunc = time => Task.FromResult((IEnumerable<ReservationItem>)new List<ReservationItem>());
            GetByTimeFunc = time => Task.FromResult((IEnumerable<ReservationItem>)new List<ReservationItem>());
        }

        public Func<ReservationItem, Task<bool>> AddReservationFunc { get; set; }
        public Func<ItemId, Task<bool>> RemoveReservationFunc { get; set; }
        public Func<ItemId, Task<bool>> RemoveAllWithShowIdAsyncFunc { get; set; }
        public Func<ItemId, Task<ReservationItem>> GetReservationFunc { get; set; }
        public Func<Task<IEnumerable<ReservationItem>>> GetAllReservationsFunc { get; set; }
        public Func<string,Task<IEnumerable<ReservationItem>>> GetByMovieFunc { get; set; }
        public Func<TimeRange, Task<IEnumerable<ReservationItem>>> GetByTimeFunc { get; set; }

        public Task<bool> AddAsync(ReservationItem item)
        {
            return AddReservationFunc(item);
        }

        public Task<bool> RemoveAsync(ItemId reservationId)
        {
            return RemoveReservationFunc(reservationId);
        }

        public Task<ReservationItem> GetByIdAsync(ItemId id)
        {
            return GetReservationFunc(id);
        }

        public Task<IEnumerable<ReservationItem>> GetAllReservationsAsync()
        {
            return GetAllReservationsFunc();
        }

        public Task<IEnumerable<ReservationItem>> GetByMovieAsync(string movieName)
        {
            return GetByMovieFunc(movieName);
        }

        public Task<IEnumerable<ReservationItem>> GetByTimeAsync(TimeRange timeRange)
        {
            return GetByTimeFunc(timeRange);
        }

        public Task<bool> RemoveAllWithShowIdAsync(ItemId showId)
        {
            return RemoveAllWithShowIdAsyncFunc(showId);
        }

    }
}