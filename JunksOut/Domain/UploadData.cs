
namespace TrashYourTreasure.Domain
{
    public class UploadData
    {
        public decimal TimeSince { get; set; }
        public int UploadDataId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public byte[] ImageData { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}