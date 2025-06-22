using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class CategoryConfiguration : IEntityTypeConfiguration<MaterialCategoryEntity>
    {
        public void Configure(EntityTypeBuilder<MaterialCategoryEntity> builder)
        {
            builder.ToTable("Categoria");             
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(c => c.Description).HasColumnName("Descripcion").IsRequired().HasMaxLength(100);
        }
    }
}
   