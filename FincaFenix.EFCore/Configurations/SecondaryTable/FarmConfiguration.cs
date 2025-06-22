using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class FarmConfiguration : IEntityTypeConfiguration<FarmEntity>
    {
        public void Configure(EntityTypeBuilder<FarmEntity> builder)
        {
            builder.ToTable("Finca");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(f => f.Name).HasColumnName("Nombre").IsRequired().HasMaxLength(100); 
            builder.Property(f => f.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);
        }
    }
}
