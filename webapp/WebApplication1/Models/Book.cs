using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("books:")]
    public class Book
    {
        [Key]
        [Column("book_idx")]
        public int id { get; set; }
        [Column("author")]
        public string Author { get; set; }
        [Column("div_code")]
        public string DivCode { get; set; }
        [Column("book_name")]
        public string BookNamer { get; set; }
        [Column("release_dt")]
        public DateTime ReleaseDt { get; set; }
        [Column("isbn")]
        public string ISBN { get; set; }
        [Column("price")]
        public decimal Price { get; set; }


    }
}
