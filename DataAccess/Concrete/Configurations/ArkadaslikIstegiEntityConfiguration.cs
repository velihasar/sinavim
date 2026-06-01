using Core.Entities.Concrete.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class ArkadaslikIstegiEntityConfiguration : BaseConfiguration<ArkadaslikIstegi>
    {
        public override void Configure(EntityTypeBuilder<ArkadaslikIstegi> builder)
        {
            builder.ToTable("ArkadaslikIstekleri");

            builder.Property(x => x.Durum).IsRequired();
            builder.Property(x => x.KullanilanDavetKodu).HasMaxLength(16);
            builder.Property(x => x.OlusturulmaTarihi).IsRequired();
            builder.Property(x => x.GonderenKabulGordu).HasDefaultValue(true);

            builder.HasIndex(x => new { x.GonderenUserId, x.HedefUserId, x.Durum });
            builder.HasIndex(x => new { x.GonderenUserId, x.Durum, x.GonderenKabulGordu });
            builder.HasIndex(x => x.HedefUserId);
            builder.HasIndex(x => x.GonderenUserId);

            builder
                .HasOne(x => x.GonderenUser)
                .WithMany()
                .HasForeignKey(x => x.GonderenUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.HedefUser)
                .WithMany()
                .HasForeignKey(x => x.HedefUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Arkadaslik)
                .WithOne(x => x.ArkadaslikIstegi)
                .HasForeignKey<Arkadaslik>(x => x.ArkadaslikIstegiId)
                .OnDelete(DeleteBehavior.SetNull);

            base.Configure(builder);
        }
    }
}
