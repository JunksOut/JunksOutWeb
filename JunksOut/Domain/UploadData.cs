
namespace JunksOut.Domain
{
    using System.Collections.Generic;
    public class UploadData
    {
        public int UploadDataId { get; set; }
        public decimal TimeSince { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public byte[] ImageData { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        //public List<UploadData> UploadDatas  { get; set; }
    }
}