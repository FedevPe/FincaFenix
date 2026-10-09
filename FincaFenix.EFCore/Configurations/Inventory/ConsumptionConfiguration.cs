using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.Inventory
{
    public class ConsumptionConfiguration : IEntityTypeConfiguration<ConsumptionEntity>
    {
        public void Configure(EntityTypeBuilder<ConsumptionEntity> builder)
        {
            builder.ToTable("Consumo");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(c => c.WorkOrderId).HasColumnName("IdOrdenTrabajo").IsRequired();
            builder.Property(c => c.MaterialId).HasColumnName("IdMaterial").IsRequired();
            builder.Property(c => c.ConsumedAmount).HasColumnName("CantidadConsumida").IsRequired().HasPrecision(18, 3).HasDefaultValue(0m);
            builder.Property(c => c.AppliedAmount).HasColumnName("CantidadAplicada").IsRequired().HasPrecision(18, 3).HasDefaultValue(0m);
            builder.Property(c => c.Unit).HasColumnName("Unidad").IsRequired(false).HasMaxLength(10);
            builder.Property(c => c.Origin).HasColumnName("Origen").IsRequired().HasMaxLength(20);
            builder.Property(c => c.UnitCost).HasColumnName("CostoUnitario").HasPrecision(18, 4).IsRequired(false);
            builder.Property(c => c.CurrencyId).HasColumnName("IdDivisa").IsRequired();
            builder.Property(c => c.CalculatedDate).HasColumnName("FechaCalculo").IsRequired().HasColumnType("datetime2(2)");
            builder.Property(c => c.UserId).HasColumnName("IdUsuario").IsRequired(false);
            builder.Property(c => c.RowVersion).HasColumnName("RowVersion").IsRowVersion().IsRequired().ValueGeneratedOnAddOrUpdate();

            builder.HasIndex(c => new { c.WorkOrderId, c.MaterialId }).IsUnique();

            builder.HasOne(c => c.WorkOrder).WithMany().HasForeignKey(c => c.WorkOrderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c => c.Material).WithMany().HasForeignKey(c => c.MaterialId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c => c.Currency).WithMany().HasForeignKey(c => c.CurrencyId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
