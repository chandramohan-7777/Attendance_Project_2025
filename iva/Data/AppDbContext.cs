using iva.Models;
using Microsoft.EntityFrameworkCore;

namespace iva.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<log> log { get; set; }
        public DbSet<log_stat> log_stat { get; set; }
        public DbSet<login> login { get; set; }
        public DbSet<office> office { get; set; }
        public DbSet<company> company { get; set; }
        public DbSet<office_scanner> office_scanner { get; set; }
        public DbSet<scanner> scanner { get; set; }
        public DbSet<employee> employee { get; set; }
        public DbSet<emp_mapping_scanner> emp_mapping_scanner { get; set; }
        public DbSet<intern> intern { get; set; }
        public DbSet<company_device> company_device { get; set; }
        public DbSet<department> department { get; set; }
        public DbSet<official_holiday> official_holiday { get; set; }
        public DbSet<manualEntryDetail> manualEntryDetail { get; set; }
        public DbSet<specialuser> specialuser { get; set; }

       
    }
}
