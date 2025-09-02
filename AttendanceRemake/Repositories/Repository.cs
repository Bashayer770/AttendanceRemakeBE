using AttendanceRemake.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace AttendanceRemake.Repositories
{
    public class Repository : IRepository
    {
        private readonly AttendanceContext _context;

        public Repository(AttendanceContext context)
        {
            _context = context;
        }


        

        public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class =>
            await _context.Set<T>().AnyAsync(predicate);   //VALIDATION matches condition and type

        public async Task<IQueryable<T>> GetAsync<T>() where T : class =>
            await Task.FromResult(_context.Set<T>().AsQueryable());  //RETURNS all Records matching type as object 

        public async Task<T?> GetByIdAsync<T>(object id) where T : class =>
            await _context.Set<T>().FindAsync(id);  // FIND BY ID 

        public async Task<T?> GetByAsync<T>(Expression<Func<T, bool>> predicate) where T : class =>
            await _context.Set<T>().FirstOrDefaultAsync(predicate); //RETURNS first match on custom condition 

        public async Task<IQueryable<T>> GetListByAsync<T>(Expression<Func<T, bool>> predicate) where T : class =>
            await Task.FromResult(_context.Set<T>().Where(predicate));  //RETURNS all records match the condition

        public async Task<bool> AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            return true;
        }//ADDS new record to DB

        public async Task UpdateAsync<T>(object id, Func<T, Task> updateFn) where T : class //FINDS AND UPDATE by ID 
        {
            var entity = await GetByIdAsync<T>(id);
            if (entity != null)
            {
                await updateFn(entity);
            }
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class  //DELETES records that matches the condition 
        {
            var entities = _context.Set<T>().Where(predicate);
            _context.Set<T>().RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();  //SAVES all changes to DB 
    }
}
