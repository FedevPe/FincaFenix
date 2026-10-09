using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.Inventory
{
    public class StockByFarmConfiguration : IEntityTypeConfiguration<StockByFarmEntity>
    {
        public void Configure(EntityTypeBuilder<StockByFarmEntity> builder)
        {
            builder.ToTable("StockPorFinca");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(s => s.MaterialId).HasColumnName("IdMaterial").IsRequired();
            builder.Property(s => s.FarmId).HasColumnName("IdFinca").IsRequired();
            builder.Property(s => s.StockFisico).HasColumnName("StockFisico").IsRequired().HasPrecision(18, 3).HasDefaultValue(0m);
            builder.Property(s => s.StockReservado).HasColumnName("StockReservado").IsRequired().HasPrecision(18, 3).HasDefaultValue(0m);
            builder.Property(s => s.StockMinimo).HasColumnName("StockMinimo").IsRequired().HasPrecision(18, 3).HasDefaultValue(0m);
            builder.Property(s => s.RowVersion).HasColumnName("RowVersion").IsRowVersion().IsRequired().ValueGeneratedOnAddOrUpdate();

            builder.HasIndex(s => new { s.MaterialId, s.FarmId }).IsUnique();

            builder.HasOne(s => s.Material).WithMany().HasForeignKey(s => s.MaterialId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s => s.Farm).WithMany().HasForeignKey(s => s.FarmId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
