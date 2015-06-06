using System.Data.Entity;

namespace JunksOut.Domain
{
    public class TytContext:DbContext
    {
        public IDbSet<UploadData> UploadData { get; set; }
    }
}