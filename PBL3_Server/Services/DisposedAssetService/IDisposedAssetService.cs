namespace PBL3_Server.Services.DisposedAssetService
{
    public interface IDisposedAssetService
    {
        Task<List<DisposedAsset>> GetAllDisposedAssets();
        Task<DisposedAsset?> GetSingleDisposedAsset(int id);
        Task<List<DisposedAsset>> AddDisposedAsset(DisposedAsset asset);
        Task<List<DisposedAsset>?> UpdateDisposedAsset(int id, DisposedAsset request);
        Task<List<DisposedAsset>?> DeleteDisposedAsset(int id);
    }
}
