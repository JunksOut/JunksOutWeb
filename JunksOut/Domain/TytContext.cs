using System.Data.Entity;

namespace TrashYourTreasure.Domain
{
    public class TytContext:DbContext
    {
        public IDbSet<UploadData> UploadData { get; set; }
    }
}