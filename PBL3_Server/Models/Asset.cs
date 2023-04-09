namespace PBL3_Server.Models
{
    public class Asset
    {
        public string AssetID { get; set; } = string.Empty;
        public string DeviceID { get; set; } = string.Empty;
        public string RoomID { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public int YearOfUse { get; set; }
        public string TechnicalSpecification { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Cost { get; set; }
        public string Status { get; set; } = string.Empty; 
        public string Notes { get; set; } = string.Empty;
    }
}
