using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.Inventory
{
    public class WorkOrderCostConfiguration : IEntityTypeConfiguration<WorkOrderCostEntity>
    {
        public void Configure(EntityTypeBuilder<WorkOrderCostEntity> builder)
        {
            builder.ToTable("CostoOrdenTrabajo");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(c => c.WorkOrderId).HasColumnName("IdOrdenTrabajo").IsRequired();
            builder.Property(c => c.MaterialId).HasColumnName("IdMaterial").IsRequired();
            builder.Property(c => c.CurrencyId).HasColumnName("IdDivisa").IsRequired();
            builder.Property(c => c.PlannedAmount).HasColumnName("CantidadPlanificada").IsRequired().HasPrecision(18, 3);
            builder.Property(c => c.UnitCost).HasColumnName("CostoUnitario").IsRequired().HasPrecision(18, 4);
            builder.Property(c => c.TotalCost).HasColumnName("CostoTotal").IsRequired().HasPrecision(18, 4);
            builder.Property(c => c.FrozenDate).HasColumnName("FechaCongelado").IsRequired().HasColumnType("datetime2(2)");

            builder.HasIndex(c => new { c.WorkOrderId, c.MaterialId }).IsUnique();

            builder.HasOne(c => c.WorkOrder).WithMany().HasForeignKey(c => c.WorkOrderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c => c.Material).WithMany().HasForeignKey(c => c.MaterialId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c => c.Currency).WithMany().HasForeignKey(c => c.CurrencyId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
