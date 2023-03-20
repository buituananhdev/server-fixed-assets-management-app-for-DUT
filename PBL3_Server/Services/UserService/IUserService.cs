namespace PBL3_Server.Services.UserService
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetSingleUser(string username);
        Task<List<User>> AddUser(User user);
        Task<List<User>?> UpdateUser(string username, User request);
        Task<List<User>?> DeleteUser(string username);
    }
}
