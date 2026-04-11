using Core.Entities.Concrete.Project;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class DersEntityConfiguration : BaseConfiguration<Ders>
    {
        public override void Configure(EntityTypeBuilder<Ders> builder)
        {
            builder.Property(x => x.IkonAnahtari).HasMaxLength(128);
            base.Configure(builder);
        }
    }
}
