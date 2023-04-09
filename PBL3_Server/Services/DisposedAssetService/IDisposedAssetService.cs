namespace PBL3_Server.Services.DisposedAssetService
{
    public interface IDisposedAssetService
    {
        Task<List<DisposedAsset>> GetAllDisposedAssets();
        Task<DisposedAsset?> GetSingleDisposedAsset(string id);
        Task<List<DisposedAsset>> AddDisposedAsset(DisposedAsset asset);
        Task<List<DisposedAsset>?> UpdateDisposedAsset(string id, DisposedAsset request);
        Task<List<DisposedAsset>?> DeleteDisposedAsset(string id);
        Task<List<DisposedAsset>?> CancelDisposeAsset(string id);
    }
}
