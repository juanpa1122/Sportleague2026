using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{
    public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
    {
        public SponsorRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<Sponsor?> GetByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();

            return await _dbSet
                .FirstOrDefaultAsync(s => s.Name.ToLower() == normalizedName);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();

            return await _dbSet.AnyAsync(s => s.Name.ToLower() == normalizedName);
        }
    }
}
