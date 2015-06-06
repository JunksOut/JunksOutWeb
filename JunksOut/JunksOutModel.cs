namespace JunksOut
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class JunksOutModel : DbContext
    {
        public JunksOutModel()
            : base("name=JunksOut")
        {
        }

        public virtual DbSet<item> items { get; set; }
        public virtual DbSet<user> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
