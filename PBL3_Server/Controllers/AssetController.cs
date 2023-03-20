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

        public AssetController(IAssetService AssetService)
        {
            _AssetService = AssetService;
        }

        [Authorize]
        [HttpGet]
        // Hàm trả về danh sách tài sản 
        public async Task<ActionResult<List<Asset>>> GetAllAssets(int pageNumber = 1, int pageSize = 10, string status = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new { message = "You don't have permission to access this page" });
            }

            var assets = await _AssetService.GetAllAssets();
            if (!string.IsNullOrEmpty(status))
            {
                assets = assets.Where(a => a.Status.ToLower() == status.ToLower()).ToList();
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
                return NotFound("Asset not found!");
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
                return NotFound("Asset not found!");

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
                return NotFound("Asset not found!");

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
                return NotFound("Asset not found!");

            return Ok(new { status = "success", data = result });
        }
    }
}
