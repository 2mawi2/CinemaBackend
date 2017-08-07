using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Model;
using Reservation.Domain;
using Show.Domain;

namespace Reservation.UnitTests
{
    public class MockShowService : IShowService
    {
        public MockShowService()
        {
            AddFunc = reservation => Task.FromResult(true);
            RemoveFunc = id => Task.FromResult(true);
            GetByIdFunc = id => Task.FromResult(new ShowItem());
            GetAllShowsFunc = () => Task.FromResult((IEnumerable<ShowItem>)new List<ShowItem>());
            GetShowsByMovieFunc = (movie) => Task.FromResult((IEnumerable<ShowItem>)new List<ShowItem>());
            TryAddReservationToShowFunc = id => Task.FromResult(true);
            TryRemoveReservationToShowFunc = id => Task.FromResult(true);
        }

        public Func<ShowItem, Task<bool>> AddFunc { get; set; }
        public Func<ItemId, Task<bool>> RemoveFunc { get; set; }
        public Func<ItemId, Task<ShowItem>> GetByIdFunc { get; set; }
        public Func<Task<IEnumerable<ShowItem>>> GetAllShowsFunc { get; set; }
        public Func<ReservationItem, Task<bool>> TryAddReservationToShowFunc { get; set; }
        public Func<ReservationItem, Task<bool>> TryRemoveReservationToShowFunc { get; set; }
        public Func<string, Task<IEnumerable<ShowItem>>> GetShowsByMovieFunc { get; set; }

        public Task<bool> AddAsync(ShowItem item)
        {
            return AddFunc(item);
        }

        public Task<bool> RemoveAsync(ItemId id)
        {
            return RemoveFunc(id);
        }

        public Task<ShowItem> GetByIdAsync(ItemId id)
        {
            return GetByIdFunc(id);
        }

        public Task<IEnumerable<ShowItem>> GetAllShowsAsync()
        {
            return GetAllShowsFunc();
        }

        public Task<bool> TryAddReservationToShowAsync(ReservationItem reservation)
        {
            return TryAddReservationToShowFunc(reservation);
        }

        public Task<IEnumerable<ShowItem>> GetShowsByMovieAsync(string movieName)
        {
            return GetShowsByMovieFunc(movieName);
        }

        public Task<bool> TryRemoveReservationFromShowAsync(ReservationItem item)
        {
            return TryRemoveReservationToShowFunc(item);
        }
    }
}