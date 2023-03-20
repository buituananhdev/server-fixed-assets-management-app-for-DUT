namespace PBL3_Server.Services.UserService
{
    public class UserService : IUserService
    {
        private static List<User> Users = new List<User>();


        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            this._context = context;
        }

        public async Task<List<User>> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Users;
        }

        public async Task<List<User>> DeleteUser(string username)
        {
            var user = await _context.Users.FindAsync(username);
            if (user is null)
                return null;
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return Users;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetSingleUser(string username)
        {
            var user = await _context.Users.FindAsync(username);
            if (user is null)
                return null;
            return user;
        }

        public async Task<List<User>> UpdateUser(string username, User request)
        {
            var user = await _context.Users.FindAsync(username);
            if (user is null)
                return null;

            user.Username = request.Username;
            user.Password = request.Password;
            user.UserRole = request.UserRole;

            await _context.SaveChangesAsync();

            return Users;
        }
    }
}
