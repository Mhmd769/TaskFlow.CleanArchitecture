using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Project?> GetProjectWithDetailsByIdAsync(Guid id)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedUsers)
                        .ThenInclude(au => au.User)
                .AsSplitQuery() // avoids multiple collection warning
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IQueryable<Project>> GetAllProjectsWithDetailsAsync()
        {
            var query = _context.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedUsers)
                        .ThenInclude(au => au.User)
                .AsSplitQuery();

            return await Task.FromResult(query);
        }
    }
}
