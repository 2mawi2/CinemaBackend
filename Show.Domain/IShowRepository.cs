using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace Show.Domain
{
    public interface IShowRepository : IRepository<ShowItem>
    {
        Task<IEnumerable<ShowItem>> GetShowsByMovieAsync(string movieName);
    }
}