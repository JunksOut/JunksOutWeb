namespace JunksOut
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("item")]
    public partial class item
    {
        public int id { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(500)]
        public string description { get; set; }

        [StringLength(50)]
        public string location { get; set; }

        [StringLength(500)]
        public string imageUrl { get; set; }

        public int userid { get; set; }
    }
}
