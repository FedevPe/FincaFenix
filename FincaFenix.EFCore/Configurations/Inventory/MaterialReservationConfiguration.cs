using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.Inventory
{
    public class MaterialReservationConfiguration : IEntityTypeConfiguration<MaterialReservationEntity>
    {
        public void Configure(EntityTypeBuilder<MaterialReservationEntity> builder)
        {
            builder.ToTable("ReservaMaterial");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(r => r.WorkOrderId).HasColumnName("IdOrdenTrabajo").IsRequired();
            builder.Property(r => r.MaterialId).HasColumnName("IdMaterial").IsRequired();
            builder.Property(r => r.FarmId).HasColumnName("IdFinca").IsRequired();
            builder.Property(r => r.ReservedAmount).HasColumnName("CantidadReservada").IsRequired().HasPrecision(18, 3);
            builder.Property(r => r.ConsumedAmount).HasColumnName("CantidadConsumida").IsRequired().HasPrecision(18, 3).HasDefaultValue(0m);
            builder.Property(r => r.State).HasColumnName("Estado").IsRequired().HasMaxLength(20);
            builder.Property(r => r.CreatedDate).HasColumnName("FechaCreacion").IsRequired().HasColumnType("datetime2(2)");
            builder.Property(r => r.ReleasedDate).HasColumnName("FechaLiberacion").HasColumnType("datetime2(2)").IsRequired(false);
            builder.Property(r => r.RowVersion).HasColumnName("RowVersion").IsRowVersion().IsRequired().ValueGeneratedOnAddOrUpdate();

            builder.HasOne(r => r.WorkOrder).WithMany().HasForeignKey(r => r.WorkOrderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.Material).WithMany().HasForeignKey(r => r.MaterialId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.Farm).WithMany().HasForeignKey(r => r.FarmId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
