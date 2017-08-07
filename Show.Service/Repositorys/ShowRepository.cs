using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Data;
using Microsoft.ServiceFabric.Data;
using Show.Domain;

namespace Show.Service.Repositorys
{
    public sealed class ShowRepository : Repository<ShowItem>, IShowRepository
    {
        public const string ShowItemDictId = "showItems";

        public ShowRepository(IReliableStateManager stateManager) : base(stateManager, ShowItemDictId)
        {
        }

        public async Task<IEnumerable<ShowItem>> GetShowsByMovieAsync(string movieName)
        {
            return await FindAsync(i => i.Movie.Equals(movieName));
        }
    }
}