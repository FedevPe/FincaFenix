using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.PrincipalTable
{
    public class DetailRecipeConfiguration : IEntityTypeConfiguration<DetailRecipeEntity>
    {
        public void Configure(EntityTypeBuilder<DetailRecipeEntity> builder)
        {
            builder.ToTable("DetalleReceta");
            builder.HasKey(dr => dr.Id);
            builder.Property(dr => dr.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(dr => dr.RecipeId).HasColumnName("IdReceta").IsRequired().HasMaxLength(20);
            builder.HasOne(dr => dr.Recipe)
                   .WithMany(r => r.DetailRecipeList)
                   .HasForeignKey(dr => dr.RecipeId)
                   .HasPrincipalKey(r => r.Id)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Property(dr => dr.MaterialId).HasColumnName("IdMaterial").IsRequired();
            builder.Property(dr => dr.AmountRequired).HasColumnName("CantRequerida").IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(dr => dr.AmountRequiredUnit).HasColumnName("UnidadCantRequerida").IsRequired().HasMaxLength(10);
            builder.Property(dr => dr.EstimatedAmount).HasColumnName("CantEstimada").IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(dr => dr.EstimatedAmountUnit).HasColumnName("UnidadCantEstimada").IsRequired().HasMaxLength(10);
            builder.Property(dr => dr.RowVersion).HasColumnName("RowVersion").IsRowVersion().IsRequired().ValueGeneratedOnAddOrUpdate();


        }
    }
}
