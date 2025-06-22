using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class FruitConfiguration : IEntityTypeConfiguration<FruitEntity>
    {
        public void Configure(EntityTypeBuilder<FruitEntity> builder)
        {
            builder.ToTable("Frutal");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(f => f.Description).HasColumnName("Frutal").IsRequired().HasMaxLength(100);
        }
    }
    
}
