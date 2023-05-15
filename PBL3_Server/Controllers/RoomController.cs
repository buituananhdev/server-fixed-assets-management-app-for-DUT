using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL3_Server.Models;
using PBL3_Server.Services.RoomService;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _RoomService;

        public RoomController(IRoomService RoomService)
        {
            _RoomService = RoomService;
        }

        [Authorize]
        [HttpGet]
        // Hàm trả về danh sách phòng 
        public async Task<ActionResult<List<Asset>>> GetAllRooms(int pageNumber = -1, int pageSize = -1, string organization_id = "", string searchQuery = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new { message = "You don't have permission to access this page" });
            }

            var rooms = await _RoomService.GetAllRooms();

            if (!string.IsNullOrEmpty(organization_id))
            {
                rooms = rooms.Where(a => a.organizationID.ToLower() == organization_id.ToLower()).ToList();
            }

            // tìm kiếm tài sản
            if (!string.IsNullOrEmpty(searchQuery))
            {
                rooms = rooms.Where(r =>
                   r.RoomID.ToLower().Contains(searchQuery.ToLower()) ||
                   r.RoomName.ToLower().Contains(searchQuery.ToLower())
                ).ToList();
            }
            if(pageNumber == -1 && pageSize == -1)
            {
                return Ok(new { status = "success", data = rooms });
            }
            else
            {
                var pagedrooms = rooms.ToPagedList(pageNumber, pageSize);

                //Tạo đối tượng paginationInfo để lưu thông tin phân trang
                var paginationInfo = new PaginationInfo
                {
                    TotalPages = pagedrooms.PageCount,
                    CurrentPage = pagedrooms.PageNumber,
                    HasPreviousPage = pagedrooms.HasPreviousPage,
                    HasNextPage = pagedrooms.HasNextPage,
                    PageSize = pagedrooms.PageSize
                };
                return Ok(new { status = "success", data = pagedrooms, meta = paginationInfo });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetSingleRoom(string id = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }

            var result = await _RoomService.GetSingleRoom(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Room not found!" });
            return Ok(new { status = "success", data = result });
        }
    }
}
