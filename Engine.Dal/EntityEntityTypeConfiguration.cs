using Engine.Field;
using Engine.Field.InternalEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Engine.Dal;

public class EntityEntityTypeConfiguration : IEntityTypeConfiguration<InternalEntity>
{
    public void Configure(EntityTypeBuilder<InternalEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasDiscriminator(x => x.Type)
            .HasValue<InternalBee>(EntityType.Bee)
            .HasValue<InternalBeeGroup>(EntityType.BeeGroup)
            .HasValue<InternalBeeHive>(EntityType.BeeHive)
            .HasValue<Flower>(EntityType.Flower)
            .IsComplete();
    }
}