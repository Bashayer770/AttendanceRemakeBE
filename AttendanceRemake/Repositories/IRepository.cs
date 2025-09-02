using System.Linq.Expressions;

namespace AttendanceRemake.Repositories
{
        public interface IRepository
        {
            Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class;  //VALIDATION matches condition and type
            Task<IQueryable<T>> GetAsync<T>() where T : class; //RETURNS all Records matching type as object 
            Task<T?> GetByIdAsync<T>(object id) where T : class; // FIND BY ID 
            Task<T?> GetByAsync<T>(Expression<Func<T, bool>> predicate) where T : class; //RETURNS first match on custom condition 
            Task<IQueryable<T>> GetListByAsync<T>(Expression<Func<T, bool>> predicate) where T : class; //RETURNS all records match the condition 
            Task<bool> AddAsync<T>(T entity) where T : class; //ADDS new record to DB
            Task UpdateAsync<T>(object id, Func<T, Task> updateFn) where T : class; //FINDS AND UPDATE by ID 
            Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class; //DELETES records that matches the condition 
            Task SaveAsync(); //SAVES all changes to DB 
        }
    }

