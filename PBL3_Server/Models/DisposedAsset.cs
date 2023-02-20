namespace PBL3_Server.Models
{
    public class DisposedAsset
    {
        public int AssetID { get; set; }
        public string DeviceID { get; set; } = string.Empty;
        public string RoomID { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public int YearOfUse { get; set; }
        public string TechnicalSpecification { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Cost { get; set; }
        public DateTime DateDisposed { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
