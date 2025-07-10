using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.BrigdeTable
{
    public class WorkOrderWorkedSectorConfiguration : IEntityTypeConfiguration<WorkOrderWorkedSectorEntity>
    {
        public void Configure(EntityTypeBuilder<WorkOrderWorkedSectorEntity> builder)
        {
            builder.ToTable("OrdenTrabajo_SectorTrabajado");
            builder.HasKey(otws => otws.Id);
            builder.Property(otws => otws.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(otws => otws.WorkOrderId).HasColumnName("IdOrdenTrabajo").IsRequired();
            builder.Property(otws => otws.SectorFarmId).HasColumnName("IdSector").IsRequired();

            builder.HasOne(otws => otws.WorkOrder) 
               .WithMany() 
               .HasForeignKey(otws => otws.WorkOrderId) 
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
