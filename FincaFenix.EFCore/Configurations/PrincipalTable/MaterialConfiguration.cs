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
            builder.Property(m => m.PackingUnit).HasColumnName("UnidadEmpaque").IsRequired(false).HasMaxLength(50);
            builder.Property(m => m.Marca).HasColumnName("Marca").IsRequired(false).HasMaxLength(50);
            builder.Property(m => m.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false); 
        }
    }
}
