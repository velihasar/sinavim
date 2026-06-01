using Core.Entities.Concrete.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class ArkadaslikEntityConfiguration : BaseConfiguration<Arkadaslik>
    {
        public override void Configure(EntityTypeBuilder<Arkadaslik> builder)
        {
            builder.ToTable("Arkadasliklar");

            builder.Property(x => x.OlusturulmaTarihi).IsRequired();

            builder.HasIndex(x => new { x.UserIdKucuk, x.UserIdBuyuk }).IsUnique();
            builder.HasIndex(x => x.UserIdKucuk);
            builder.HasIndex(x => x.UserIdBuyuk);

            builder
                .HasOne(x => x.UserKucuk)
                .WithMany()
                .HasForeignKey(x => x.UserIdKucuk)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.UserBuyuk)
                .WithMany()
                .HasForeignKey(x => x.UserIdBuyuk)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasCheckConstraint(
                "CK_Arkadaslik_UserIdSiralama",
                "\"UserIdKucuk\" < \"UserIdBuyuk\"");

            base.Configure(builder);
        }
    }
}
