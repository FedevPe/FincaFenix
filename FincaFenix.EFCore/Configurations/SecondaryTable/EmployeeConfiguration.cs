using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeEntity>
    {
        public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
        {
            builder.ToTable("Empleado");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(e => e.Name).HasColumnName("Nombre").IsRequired().HasMaxLength(50);
            builder.Property(e => e.LastName).HasColumnName("Apellido").IsRequired().HasMaxLength(50);
            builder.Property(e => e.IsDeleted).HasColumnName("IsDeleted").IsRequired().HasDefaultValue(false);
        }
    }
}
