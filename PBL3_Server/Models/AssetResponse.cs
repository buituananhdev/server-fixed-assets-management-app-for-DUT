namespace PBL3_Server.Models
{
    public class AssetResponse
    {

        public string status { get; set; }
        public List<Asset> data { get; set; }

        public AssetResponse(string v, List<Asset> assets)
        {
            this.status = v;
            this.data = assets;
        }
    }
}
