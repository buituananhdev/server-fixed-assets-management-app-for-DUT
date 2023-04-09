namespace PBL3_Server.Services.RoomService
{
    public interface IAssetService
    {
        Task<List<Asset>> GetAllAssets();
        Task<Asset?> GetSingleAsset(string id);
        Task<List<Asset>> AddAsset(Asset asset);
        Task<List<Asset>?> UpdateAsset(string id, Asset request);
        Task<List<Asset>?> DeleteAsset(string id);
        Task<List<Asset>?> DisposedAsset(string id);
    }
}
