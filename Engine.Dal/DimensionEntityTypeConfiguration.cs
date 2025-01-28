using Engine.Field.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engine.Dal;

public class DimensionEntityTypeConfiguration : IEntityTypeConfiguration<Dimension>
{
    public void Configure(EntityTypeBuilder<Dimension> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasMany(x => x.Parts).WithOne(x => x.Dimension).HasForeignKey(x => x.DimensionId).OnDelete(DeleteBehavior.Cascade);
    }
}