using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class FruitVarietyConfiguration : IEntityTypeConfiguration<FruitVarietyEntity>
    {
        public void Configure(EntityTypeBuilder<FruitVarietyEntity> builder)
        {
            builder.ToTable("VariedadFrutal");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(v => v.Description).HasColumnName("VariedadFruta").IsRequired().HasMaxLength(100);
            builder.Property(v => v.FruitId).HasColumnName("IdFruta").IsRequired();
        }
    }
}
