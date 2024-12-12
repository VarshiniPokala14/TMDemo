namespace TrekMasters.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserDetail> _userManager;
        public UserRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<UserDetail> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<UserDetail> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
        public string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(user);
        }
        public async Task<UserDetail> GetUserAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> UpdateUserAsync(UserDetail user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<EmergencyContact> GetEmergencyContactAsync(string userId)
        {
            return await _context.EmergencyContacts
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task AddEmergencyContactAsync(EmergencyContact emergencyContact)
        {
            _context.EmergencyContacts.Add(emergencyContact);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmergencyContactAsync(EmergencyContact emergencyContact)
        {
            _context.EmergencyContacts.Update(emergencyContact);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Trek)
                .Where(b => b.UserId == userId && (b.IsCancelled == false || b.IsCancelled == null))
                .ToListAsync();
        }
    }
}
