using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions options) : base(options) { 
        
            // 자동생성. 내용 없음
        }

        public DbSet<Book> Books => Set<Book>();
    }
}
