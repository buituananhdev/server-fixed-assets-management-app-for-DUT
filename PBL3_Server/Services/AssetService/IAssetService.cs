namespace PBL3_Server.Services.RoomService
{
    public interface IRoomService
    {
        Task<List<Asset>> GetAllAssets();
        Task<Asset?> GetSingleAsset(int id);
        Task<List<Asset>> AddAsset(Asset asset);
        Task<List<Asset>?> UpdateAsset(int id, Asset request);
        Task<List<Asset>?> DeleteAsset(int id);
    }
}
