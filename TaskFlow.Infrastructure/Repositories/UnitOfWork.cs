// Infrastructure/Repositories/UnitOfWork.cs
using System;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IRepository<User> Users { get; }
    public IRepository<Project> Projects { get; }
    public IRepository<TaskItem> Tasks { get; }
    public IRepository<Comment> Comments { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new GenericRepository<User>(_context);
        Projects = new GenericRepository<Project>(_context);
        Tasks = new GenericRepository<TaskItem>(_context);
        Comments = new GenericRepository<Comment>(_context);
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
