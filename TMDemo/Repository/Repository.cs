namespace TrekMasters.Repository
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<bool> AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            return await _context.SaveChangesAsync()>0;
        }

        public async Task<bool> UpdateAsync<T>(T entity) where T : class
        {
             _context.Set<T>().Update(entity);
            return await _context.SaveChangesAsync()>0;
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
