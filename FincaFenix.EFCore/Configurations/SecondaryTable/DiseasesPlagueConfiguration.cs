using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class DiseasesPlagueConfiguration : IEntityTypeConfiguration<DiseasePlagueEntity>
    {
        public void Configure(EntityTypeBuilder<DiseasePlagueEntity> builder)
        {
            builder.ToTable("EnfermedadPlaga");
            builder.HasKey(dp => dp.Id);
            builder.Property(dp => dp.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(dp => dp.Description).HasColumnName("Descripcion").IsRequired().HasMaxLength(100);
        }
    }
}
