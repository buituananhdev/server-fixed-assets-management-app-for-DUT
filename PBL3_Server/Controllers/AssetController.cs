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
    [Route("api/asset")]
    [ApiController]
    public class AssetController : ControllerBase
    {

        private readonly IAssetService _AssetService;

        public AssetController(IAssetService AssetService)
        {
            _AssetService = AssetService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<List<Asset>>> GetAllAssets(int pageNumber = 1, int pageSize = 10, string status = "")
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Ok(new { message = "You don't have permission to access this page" });
            }

            var assets = await _AssetService.GetAllAssets();
            var pagedAssets = assets.ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(status))
            {
                assets = assets.Where(a => a.Status.ToLower() == status.ToLower()).ToList();
            }

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

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Asset>> GetSingleAsset(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Bạn không có quyền truy cập.");
            }
            var result = await _AssetService.GetSingleAsset(id);
            if (result is null)
                return NotFound("Asset not found!");
            return Ok(new { status = "success", data = result});
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<Asset>>> AddAsset(Asset asset)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Bạn không có quyền truy cập.");
            }
            var result = await _AssetService.AddAsset(asset);
            return Ok(new { status = "success", data = result });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<Asset>>> UpdateAsset(int id, Asset request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Bạn không có quyền truy cập.");
            }
            var result = await _AssetService.UpdateAsset(id, request);
            if (result is null)
                return NotFound("Asset not found!");

            return Ok(new { status = "success", data = result });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Asset>>> DeleteAsset(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Bạn không có quyền truy cập.");
            }
            var result = await _AssetService.DeleteAsset(id);
            if (result is null)
                return NotFound("Asset not found!");

            return Ok(new { status = "success", data = result });
        }

        
    }
}
