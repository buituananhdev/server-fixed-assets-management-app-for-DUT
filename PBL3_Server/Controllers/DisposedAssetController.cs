using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL3_Server.Models;
using PBL3_Server.Services.AssetService;
using PBL3_Server.Services.DisposedAssetService;
using PBL3_Server.Services.RoomService;
using System.Data;
using X.PagedList;

namespace PBL3_Server.Controllers
{
    [Route("api/disposed_asset")]
    [ApiController]
    public class DisposedAssetController : ControllerBase
    {
        private readonly IDisposedAssetService _DisposedAssetService;
        private readonly IRoomService _RoomService;

        public DisposedAssetController(IDisposedAssetService DisposedAssetService, IRoomService RoomService)
        {
            _DisposedAssetService = DisposedAssetService;
            _RoomService = RoomService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<DisposedAsset>>> GetAllDisposedAssets(int pageNumber = 1, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, string organization_id = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var assets = await _DisposedAssetService.GetAllDisposedAssets();

            // Lọc tài sản theo mã khoa của phòng
            if (!string.IsNullOrEmpty(organization_id))
            {
                var rooms = await _RoomService.GetAllRooms();
                rooms = rooms.Where(r => r.organizationID.ToLower() == organization_id.ToLower()).ToList();
                assets = assets.Where(a => rooms.Any(r => r.RoomID == a.RoomID)).ToList();
            }

            // lọc tài sản theo ngày thanh lý nằm trong start date và end date
            if (startDate.HasValue && endDate.HasValue)
            {
                assets = assets.Where(a => a.DateDisposed >= startDate.Value && a.DateDisposed <= endDate.Value).ToList();
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
        public async Task<ActionResult<DisposedAsset>> GetSingleDisposedAsset(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.GetSingleDisposedAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<DisposedAsset>>> AddDisposedAsset(DisposedAsset asset)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.AddDisposedAsset(asset);
            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<DisposedAsset>>> UpdateDisposedAsset(int id, DisposedAsset request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.UpdateDisposedAsset(id, request);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<DisposedAsset>>> DeleteDisposedAsset(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.DeleteDisposedAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }

        [Authorize]
        [HttpPost("{id}")]
        // Hàm hủy thanh lý tài sản theo ID
        public async Task<ActionResult<List<DisposedAsset>>> CancelDisposeAsset(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var result = await _DisposedAssetService.CancelDisposeAsset(id);
            if (result is null)
                return NotFound(new { status = "failure", message = "Asset not found!" });

            return Ok(new { status = "success", data = result });
        }
    }
}
