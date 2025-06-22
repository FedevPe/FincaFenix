using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.PrincipalTable
{
    public class RecipeConfiguration : IEntityTypeConfiguration<RecipeEntity>
    {
        public void Configure(EntityTypeBuilder<RecipeEntity> builder)
        {
            builder.ToTable("Receta");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(r => r.NumRecipe).HasColumnName("NumReceta").IsRequired().HasMaxLength(20);
            builder.Property(r => r.MachineId).HasColumnName("IdMaquina").IsRequired();
            builder.Property(r => r.Dosage).HasColumnName("Dosis").IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(r => r.DosageUnit).HasColumnName("UnidadDosis").IsRequired().HasMaxLength(10);
            builder.Property(r => r.State).HasColumnName("Estado").IsRequired().HasMaxLength(50);
            builder.Property(r => r.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);
            builder.Property(r => r.RowVersion).HasColumnName("RowVersion").IsRowVersion().IsRequired().ValueGeneratedOnAddOrUpdate();
        }
    }
}
