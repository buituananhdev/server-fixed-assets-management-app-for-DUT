using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_Server.Models;
using PBL3_Server.Services.RoomService;
using System.Data;

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
        public async Task<ActionResult<List<Asset>>> GetAllAssets()
        {
            return await _AssetService.GetAllAssets();
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Asset>> GetSingleAsset(int id)
        {
            var result = await _AssetService.GetSingleAsset(id);
            if (result is null)
                return NotFound("Asset not found!");

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<List<Asset>>> AddAsset(Asset asset)
        {
            var result = await _AssetService.AddAsset(asset);
            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<List<Asset>>> UpdateAsset(int id, Asset request)
        {
            var result = await _AssetService.UpdateAsset(id, request);
            if (result is null)
                return NotFound("Asset not found!");

            return Ok(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Asset>>> DeleteAsset(int id)
        {
            var result = await _AssetService.DeleteAsset(id);
            if (result is null)
                return NotFound("Asset not found!");

            return Ok(result);
        }

        /*
         [HttpPost]
        [Route("dispose-asset")]
        public IActionResult DisposeAsset(int assetID)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var dateDisposed = DateTime.Now;

                    var asset = _context.Assets.FirstOrDefault(a => a.AssetID == assetID);

                    if (asset == null)
                    {
                        return BadRequest("Asset not found");
                    }

                    var disposedAsset = new DisposedAsset
                    {
                        AssetID = asset.AssetID,
                        RoomID = asset.RoomID,
                        AssetName = asset.AssetName,
                        YearOfUse = asset.YearOfUse,
                        TechnicalSpecification = asset.TechnicalSpecification,
                        Quantity = asset.Quantity,
                        Cost = asset.Cost,
                        DateDisposed = dateDisposed,
                        Notes = asset.Notes
                    };

                    _context.DisposedAssets.Add(disposedAsset);
                    _context.Assets.Remove(asset);
                    _context.SaveChanges();

                    transaction.Commit();

                    return Ok();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(500, ex.Message);
                }
            }
        }
         */
    }
}
