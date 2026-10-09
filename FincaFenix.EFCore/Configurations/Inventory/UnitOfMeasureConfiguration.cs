using FincaFenix.Entities.POCOEntities;
using FincaFenix.Entities.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.Inventory
{
    public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasureEntity>
    {
        public void Configure(EntityTypeBuilder<UnitOfMeasureEntity> builder)
        {
            builder.ToTable("UnidadMedida");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(u => u.Description).HasColumnName("Descripcion").IsRequired().HasMaxLength(50);
            builder.Property(u => u.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);

            builder.HasData(UnitOfMeasureCatalog.Seed.Select(u => new UnitOfMeasureEntity
            {
                Id = u.Id,
                Description = u.Description,
                IsDeleted = false
            }));
        }
    }
}
