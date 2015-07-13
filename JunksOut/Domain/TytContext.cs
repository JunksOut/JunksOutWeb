using System.Data.Entity;

namespace JunksOut.Domain
{
    public class TytContext:DbContext
    {

        public TytContext()
            : base("name=TytContext.mdf")
        {
        }

        public IDbSet<UploadData> UploadData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UploadData>().ToTable("UploadData");
            
            base.OnModelCreating(modelBuilder);
        }




    }
}