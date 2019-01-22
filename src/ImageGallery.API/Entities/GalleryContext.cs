using Microsoft.EntityFrameworkCore;

namespace ImageGallery.API.Entities
{
    public class GalleryContextBase : DbContext
    {
        public GalleryContextBase(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }
    }

    public class GalleryContext : GalleryContextBase
    {
        public GalleryContext(DbContextOptions<GalleryContext> options)
            : base(options)
        {
        }
    }

    public class MigrationGalleryContext : GalleryContextBase
    {
        public MigrationGalleryContext(DbContextOptions<MigrationGalleryContext> options)
            : base(options)
        {
        }
    }
}