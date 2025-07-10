using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class MachineConfiguration : IEntityTypeConfiguration<MachineEntity>
    {
        public void Configure(EntityTypeBuilder<MachineEntity> builder)
        {
            builder.ToTable("Maquinaria");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(m => m.Name).HasColumnName("Nombre").IsRequired().HasMaxLength(100);
            builder.Property(m => m.Capacity).HasColumnName("Capacidad").IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(m => m.CapacityUnit).HasColumnName("UnidadCapacidad").IsRequired(false).HasMaxLength(50);
            builder.Property(m => m.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);
        }
    }
}
