using Core.Entities.Concrete.Project;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class KullaniciDersNetHedefiEntityConfiguration : BaseConfiguration<KullaniciDersNetHedefi>
    {
        public override void Configure(EntityTypeBuilder<KullaniciDersNetHedefi> builder)
        {
            builder.HasIndex(x => new { x.UserId, x.DersId }).IsUnique();

            base.Configure(builder);
        }
    }
}
