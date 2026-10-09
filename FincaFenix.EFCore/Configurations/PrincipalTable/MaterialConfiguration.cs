using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.PrincipalTable
{
    public class MaterialConfiguration : IEntityTypeConfiguration<MaterialEntity>
    {
        public void Configure(EntityTypeBuilder<MaterialEntity> builder)
        {
            builder.ToTable("Material");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(m => m.ArticleName).HasColumnName("NombreArticulo").IsRequired().HasMaxLength(100);
            builder.Property(m => m.CommercialName).HasColumnName("NombreComercial").IsRequired().HasMaxLength(100);
            builder.Property(m => m.CategoryId).HasColumnName("IdCategoria").IsRequired();
            builder.Property(m => m.Brand).HasColumnName("Marca").IsRequired(false).HasMaxLength(50);
            builder.Property(m => m.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);
            builder.Property(m => m.CodeSap).HasColumnName("CodigoSAP").IsRequired(false).HasMaxLength(30);
            builder.Property(m => m.DescriptionSap).HasColumnName("DescripcionSAP").IsRequired(false).HasMaxLength(100);
            builder.Property(m => m.Description).HasColumnName("Descripcion").IsRequired(false).HasMaxLength(30);
            builder.Property(m => m.UnitOfMeasureId).HasColumnName("IdUnidadMedida").IsRequired();
            builder.Property(m => m.ReferenceCost).HasColumnName("CostoReferencia").HasPrecision(18, 4).IsRequired(false);
            builder.Property(m => m.CurrencyId).HasColumnName("IdDivisa").IsRequired().HasDefaultValue(1);

            builder.HasOne(m => m.UnitOfMeasure).WithMany().HasForeignKey(m => m.UnitOfMeasureId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(m => m.Currency).WithMany().HasForeignKey(m => m.CurrencyId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
