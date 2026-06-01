using Core.Entities.Concrete;
using Core.Entities.Concrete.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class KullaniciDavetKoduEntityConfiguration : IEntityTypeConfiguration<KullaniciDavetKodu>
    {
        public void Configure(EntityTypeBuilder<KullaniciDavetKodu> builder)
        {
            builder.ToTable("KullaniciDavetKodlari");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Kod).HasMaxLength(16).IsRequired();
            builder.Property(x => x.OlusturulmaTarihi).IsRequired();

            builder.HasIndex(x => x.Kod).IsUnique();
            builder.HasIndex(x => x.UserId).IsUnique();

            builder
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
