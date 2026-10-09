using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.Inventory
{
    public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovementEntity>
    {
        public void Configure(EntityTypeBuilder<InventoryMovementEntity> builder)
        {
            builder.ToTable("MovimientoInventario");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(m => m.MaterialId).HasColumnName("IdMaterial").IsRequired();
            builder.Property(m => m.FarmId).HasColumnName("IdFinca").IsRequired();
            builder.Property(m => m.MovementType).HasColumnName("TipoMovimiento").IsRequired().HasMaxLength(20);
            builder.Property(m => m.Amount).HasColumnName("Cantidad").IsRequired().HasPrecision(18, 3);
            builder.Property(m => m.UnitCost).HasColumnName("CostoUnitario").HasPrecision(18, 4).IsRequired(false);
            builder.Property(m => m.TotalCost).HasColumnName("CostoTotal").HasPrecision(18, 4).IsRequired(false);
            builder.Property(m => m.CurrencyId).HasColumnName("IdDivisa").IsRequired();
            builder.Property(m => m.PreviousStock).HasColumnName("StockAnterior").IsRequired().HasPrecision(18, 3);
            builder.Property(m => m.ResultingStock).HasColumnName("StockResultante").IsRequired().HasPrecision(18, 3);
            builder.Property(m => m.Date).HasColumnName("Fecha").IsRequired().HasColumnType("datetime2(2)");
            builder.Property(m => m.UserId).HasColumnName("IdUsuario").IsRequired(false);
            builder.Property(m => m.Observations).HasColumnName("Observaciones").IsRequired(false).HasMaxLength(500);
            builder.Property(m => m.Origin).HasColumnName("Origen").IsRequired().HasMaxLength(20);
            builder.Property(m => m.WorkOrderId).HasColumnName("IdOrdenTrabajo").IsRequired(false);

            builder.HasIndex(m => new { m.MaterialId, m.Date });
            builder.HasIndex(m => new { m.FarmId, m.Date });
            builder.HasIndex(m => m.WorkOrderId);

            builder.HasOne(m => m.Material).WithMany().HasForeignKey(m => m.MaterialId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.Farm).WithMany().HasForeignKey(m => m.FarmId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.WorkOrder).WithMany().HasForeignKey(m => m.WorkOrderId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(m => m.Currency).WithMany().HasForeignKey(m => m.CurrencyId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
