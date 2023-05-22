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

        public async Task<List<User>> DeleteUser(string userID)
        {
            var user = await _context.Users.FindAsync(userID);
            if (user is null)
                return null;
            var tokensToDelete = _context.Tokens.Where(t => t.UserID == userID);
            _context.Tokens.RemoveRange(tokensToDelete);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Users;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetSingleUser(string userID)
        {
            var user = await _context.Users.FindAsync(userID);
            if (user is null)
                return null;
            return user;
        }

        public async Task<List<User>> UpdateUser(string userID, User request)
        {
            var user = await _context.Users.FindAsync(userID);
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
