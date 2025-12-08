using System;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;

    // Generic repositories
    public IRepository<User> Users { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<TaskItem> Tasks { get; }
    public IRepository<Comment> Comments { get; }

    // Specialized repository
    public IProjectRepository ProjectsWithDetails { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        // Generic repositories
        Users = new GenericRepository<User>(_context);
        Projects = new GenericRepository<Project>(_context);
        Tasks = new GenericRepository<TaskItem>(_context);
        Comments = new GenericRepository<Comment>(_context);

        // Specialized Project repository
        ProjectsWithDetails = new ProjectRepository(_context);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
