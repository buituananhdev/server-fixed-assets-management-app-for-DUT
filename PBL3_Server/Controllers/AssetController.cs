using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Server.Models;
using PBL3_Server.Services.RoomService;
using System.Data;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/asset")] // thiết lập đường dẫn tương đối cho API
    [ApiController] // ApiController đảm bảo rằng nếu một truy vấn không hợp lệ được thực hiện, sẽ trả về kết quả BadRequest (400)
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _AssetService;
        private readonly IRoomService _RoomService;

        public AssetController(IAssetService AssetService, IRoomService RoomService)
        {
            _AssetService = AssetService;
            _RoomService = RoomService;
        }

        [Authorize]
        [HttpGet]
        // Hàm trả về danh sách tài sản 
        public async Task<ActionResult<List<Asset>>> GetAllAssets(int pageNumber = 1, int pageSize = 10, string status = "", string room_id = "", string organization_id = "", string searchQuery = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new { message = "You don't have permission to access this page" });
            }
            
            // Lấy danh sách tài sản và join với bảng Room để lấy thông tin phòng tài sản

            var assets = await _AssetService.GetAllAssets();

            // Lọc tài sản theo mã khoa của phòng
            if (!string.IsNullOrEmpty(organization_id))
            {
                var rooms = await _RoomService.GetAllRooms();
                rooms = rooms.Where(r => r.organizationID.ToLower() == organization_id.ToLower()).ToList();
                assets = assets.Where(a => rooms.Any(r => r.RoomID == a.RoomID)).ToList();
            }

            // Lọc tài sản theo trạng thái nếu status khác rỗng
            if (!string.IsNullOrEmpty(status))
            {
                assets = assets.Where(a => a.Status.ToLower() == status.ToLower()).ToList();
            }

            // Lọc tài sản theo mã phòng nếu room_id khác rỗng
            if (!string.IsNullOrEmpty(room_id))
            {
                assets = assets.Where(a => a.RoomID.ToLower() == room_id.ToLower()).ToList();
            }

            // tìm kiếm tài sản
            if (!string.IsNullOrEmpty(searchQuery))
            {
                assets = assets.Where(a =>
                    a.AssetID.ToString().ToLower() == searchQuery.ToLower() ||
                    a.DeviceID.ToLower().Contains(searchQuery.ToLower()) ||
                    a.AssetName.ToLower().Contains(searchQuery.ToLower()) ||
                    a.Cost.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    a.RoomID.ToLower().Contains(searchQuery.ToLower()) ||
                    a.YearOfUse.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    a.Status.ToLower().Contains(searchQuery.ToLower()) ||
                    a.Quantity.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    a.TechnicalSpecification.ToLower().Contains(searchQuery.ToLower()) ||
                    a.Quantity.ToString().ToLower().Contains(searchQuery.ToLower()) ||
                    a.Notes.ToLower().Contains(searchQuery.ToLower())
                ).ToList();
            }

            var pagedAssets = assets.ToPagedList(pageNumber, pageSize);


            //Tạo đối tượng paginationInfo để lưu thông tin phân trang
            var paginationInfo = new PaginationInfo
            {
                TotalPages = pagedAssets.PageCount,
                CurrentPage = pagedAssets.PageNumber,
                HasPreviousPage = pagedAssets.HasPreviousPage,
                HasNextPage = pagedAssets.HasNextPage,
                PageSize = pagedAssets.PageSize
            };
            return Ok(new { status = "success", data = pagedAssets, meta = paginationInfo });
        }

        [Authorize]
        [HttpGet("{id}")]
        // Hàm trả về thông tin của tài sản qua ID
        public async Task<ActionResult<Asset>> GetSingleAsset(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.GetSingleAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });
            return Ok(new { status = "success", data = result});
        }

        [Authorize]
        [HttpPost]
        // Hàm thêm tài sản
        public async Task<ActionResult<List<Asset>>> AddAsset(Asset asset)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.AddAsset(asset);
            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPut("{id}")]
        //Hàm cập nhật tài sản
        public async Task<ActionResult<List<Asset>>> UpdateAsset(int id, Asset request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.UpdateAsset(id, request);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        // Hàm xóa tài sản theo ID
        public async Task<ActionResult<List<Asset>>> DeleteAsset(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.DeleteAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPost("{id}")]
        // Hàm thanh lý tài sản theo ID
        public async Task<ActionResult<List<Asset>>> DisposedAsset(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _AssetService.DisposedAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }
    }
}
