using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL3_Server.Services.RoomService;
using PBL3_Server.Services.UserService;
using System.Data;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;

        public UserController(IUserService UserService)
        {
            _UserService = UserService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        // Hàm trả về danh sách user 
        public async Task<ActionResult<List<User>>> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }

            var users = await _UserService.GetAllUsers();
            var pagedUsers = users.ToPagedList(pageNumber, pageSize);

            //Tạo đối tượng paginationInfo để lưu thông tin phân trang
            var paginationInfo = new PaginationInfo
            {
                TotalPages = pagedUsers.PageCount,
                CurrentPage = pagedUsers.PageNumber,
                HasPreviousPage = pagedUsers.HasPreviousPage,
                HasNextPage = pagedUsers.HasNextPage,
                PageSize = pagedUsers.PageSize
            };
            return Ok(new { status = "success", data = pagedUsers, meta = paginationInfo });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{username}")]
        // Hàm trả về thông tin của user qua ID
        public async Task<ActionResult<User>> GetSingleUser(string username)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _UserService.GetSingleUser(username);
            if (result is null)
                return NotFound(new { status = "failure", message = "User not found!" });
            return Ok(new { status = "success", data = result });
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        // Hàm thêm user
        public async Task<ActionResult<List<User>>> AddUser(User user)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;
            var result = await _UserService.AddUser(user);
            return Ok(new { status = "success", data = result });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{username}")]
        //Hàm cập nhật user
        public async Task<ActionResult<List<User>>> UpdateUser(string username, User request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _UserService.UpdateUser(username, request);
            if (result is null)
                return NotFound(new { status = "failure", message = "User not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{username}")]
        // Hàm xóa user theo ID
        public async Task<ActionResult<List<User>>> DeleteUser(string username)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _UserService.DeleteUser(username);
            if (result is null)
                return NotFound(new { status = "failure" , message = "User not found!" });

            return Ok(new { status = "success", data = result });
        }
    }
}
