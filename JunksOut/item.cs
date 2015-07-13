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

        [StringLength(500)]
        public string address { get; set; }

        [StringLength(500)]
        public string location { get; set; }

        [StringLength(500)]
        public string description { get; set; }

        public int userid { get; set; }

        [StringLength(500)]
        public string tags { get; set; }

        //[StringLength(500)]
        //public string imageUrl { get; set; }
    }
}
