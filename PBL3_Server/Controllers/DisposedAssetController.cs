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

        public DisposedAssetController(IDisposedAssetService DisposedAssetService)
        {
            _DisposedAssetService = DisposedAssetService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<DisposedAsset>>> GetAllDisposedAssets(int pageNumber = 1, int pageSize = 10, DateTime? dateDisposed = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "You don't have permission to access this page" });
            }
            var assets = await _DisposedAssetService.GetAllDisposedAssets();
            var pagedAssets = assets.ToPagedList(pageNumber, pageSize);
            if (dateDisposed.HasValue)
            {
                assets = assets.Where(a => a.DateDisposed == dateDisposed.Value).ToList();
            }

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
