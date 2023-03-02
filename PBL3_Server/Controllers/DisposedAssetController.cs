using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PBL3_Server.Models;
using PBL3_Server.Services.DisposedAssetService;
using PBL3_Server.Services.RoomService;
using System.Data;

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

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<List<DisposedAsset>>> GetAllDisposedAssets()
        {
            return await _DisposedAssetService.GetAllDisposedAssets();
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DisposedAsset>> GetSingleDisposedAsset(int id)
        {
            var result = await _DisposedAssetService.GetSingleDisposedAsset(id);
            if (result is null)
                return NotFound("Asset not found!");

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<DisposedAsset>>> AddDisposedAsset(DisposedAsset asset)
        {
            var result = await _DisposedAssetService.AddDisposedAsset(asset);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<DisposedAsset>>> UpdateDisposedAsset(int id, DisposedAsset request)
        {
            var result = await _DisposedAssetService.UpdateDisposedAsset(id, request);
            if (result is null)
                return NotFound("Asset not found!");

            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<DisposedAsset>>> DeleteDisposedAsset(int id)
        {
            var result = await _DisposedAssetService.DeleteDisposedAsset(id);
            if (result is null)
                return NotFound("Asset not found!");

            return Ok(result);
        }
    }
}
