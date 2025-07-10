using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.BrigdeTable
{
    public class FarmEmployeeConfiguration : IEntityTypeConfiguration<Employee_FarmEntity>
    {
        public void Configure(EntityTypeBuilder<Employee_FarmEntity> builder)
        {
            builder.ToTable("Empleado_Finca");
            builder.HasKey(ef => ef.Id);
            builder.Property(ef => ef.Id).HasColumnName("IdEmpleadoFinca").IsRequired().ValueGeneratedOnAdd();
            builder.Property(ef => ef.EmployeeId).HasColumnName("IdEmpleado").IsRequired();
            builder.HasOne(ef => ef.Employee)
                .WithMany(e => e.EmployeeList)
                .HasForeignKey(ef => ef.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Property(ef => ef.FarmId).HasColumnName("IdFinca").IsRequired();
            builder.HasOne(ef => ef.Farm)
                .WithMany(f => f.FarmList)
                .HasForeignKey(ef => ef.FarmId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
