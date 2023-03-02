using Microsoft.EntityFrameworkCore;
using PBL3_Server.Models;

namespace PBL3_Server.Services.DisposedAssetService
{
    public class DisposedAssetService : IDisposedAssetService
    {
        private static List<DisposedAsset> DisposedAssets = new List<DisposedAsset>
        {
            new DisposedAsset
            {
                AssetID = 0,
                DeviceID = "aa",
                RoomID = "f101",
                AssetName = "Name",
                YearOfUse = 2003,
                TechnicalSpecification = "ggg",
                Quantity = 0,
                Cost= 0,
                DateDisposed = DateTime.Now,
                Notes = "ssdfsdf"
            }
        };


        private readonly DataContext _context;

        public DisposedAssetService(DataContext context)
        {
            this._context = context;
        }

        public async Task<List<DisposedAsset>> AddDisposedAsset(DisposedAsset asset)
        {
            _context.DisposedAssets.Add(asset);
            await _context.SaveChangesAsync();
            return DisposedAssets;
        }

        public async Task<List<DisposedAsset>?> DeleteDisposedAsset(int id)
        {
            var asset = await _context.DisposedAssets.FindAsync(id);
            if (asset is null)
                return null;
            _context.Remove(asset);
            await _context.SaveChangesAsync();
            return DisposedAssets;
        }

        public async Task<List<DisposedAsset>> GetAllDisposedAssets()
        {
            var assets = await _context.DisposedAssets.ToListAsync();
            return assets;
        }

        public async Task<DisposedAsset?> GetSingleDisposedAsset(int id)
        {
            var asset = await _context.DisposedAssets.FindAsync(id);
            if (asset is null)
                return null;
            return asset;
        }

        public async Task<List<DisposedAsset>?> UpdateDisposedAsset(int id, DisposedAsset request)
        {
            var asset = await _context.DisposedAssets.FindAsync(id);
            if (asset is null)
                return null;

            asset.AssetID = request.AssetID;
            asset.DeviceID = request.DeviceID;
            asset.RoomID = request.RoomID;
            asset.AssetName = request.AssetName;
            asset.YearOfUse = request.YearOfUse;
            asset.TechnicalSpecification = request.TechnicalSpecification;
            asset.Quantity = request.Quantity;
            asset.Cost = request.Cost;
            asset.DateDisposed = request.DateDisposed;
            asset.Notes = request.Notes;

            await _context.SaveChangesAsync();

            return DisposedAssets;
        }
    }
}
