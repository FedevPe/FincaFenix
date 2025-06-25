using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.PrincipalTable
{
    public class WorkOrderDetailConfiguration : IEntityTypeConfiguration<DetailWorkOrderEntity>
    {
        public void Configure(EntityTypeBuilder<DetailWorkOrderEntity> builder)
        {
            builder.ToTable("DetalleOrdenTrabajo");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(d => d.WorkOrderId).HasColumnName("IdOrdenTrabajo").IsRequired();
            builder.HasOne(d => d.WorkOrder)
                .WithMany(w => w.DetailWorkOrderList)
                .HasForeignKey(d => d.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(d => d.EmployeeId).HasColumnName("IdEmpleado").IsRequired();
            builder.Property(d => d.SectorWorkedId).HasColumnName("IdSectorTrabajado");
            builder.Property(d => d.WorkedHours).HasColumnName("HorasTrabajadas").IsRequired().HasPrecision(5, 2);
            builder.Property(d => d.Performance).HasColumnName("Rendimiento").IsRequired().HasPrecision(5, 2);
            builder.Property(d => d.Description).HasColumnName("Descripcion").IsRequired().HasMaxLength(500);
            builder.Property(d => d.RowVersion).HasColumnName("RowVersion").IsRowVersion().IsRequired().ValueGeneratedOnAddOrUpdate();

        }
    }
}
