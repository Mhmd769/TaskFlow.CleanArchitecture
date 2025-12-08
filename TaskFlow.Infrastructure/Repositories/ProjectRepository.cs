using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetProjectWithDetailsByIdAsync(Guid id)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Tasks)
                    .ThenInclude(t=>t.AssignedUsers)
                        .ThenInclude(u=>u.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }

}
