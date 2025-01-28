using Engine.Field.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engine.Dal;

public class DimensionPartEntityTypeConfiguration : IEntityTypeConfiguration<DimensionPart>
{
    public void Configure(EntityTypeBuilder<DimensionPart> builder)
    {
        builder.HasKey(x => new { x.X, x.Y });
        builder.OwnsMany(x => x.Units);
    }
}