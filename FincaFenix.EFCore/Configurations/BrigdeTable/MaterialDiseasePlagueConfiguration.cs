using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.BrigdeTable
{
    public class MaterialDiseasePlagueConfiguration : IEntityTypeConfiguration<DiseasePlague_MaterialEntity>
    {
        public void Configure(EntityTypeBuilder<DiseasePlague_MaterialEntity> builder)
        {
            builder.ToTable("EnfermedadPlaga_Material");
            builder.HasKey(epm => epm.Id);
            builder.Property(epm => epm.Id).HasColumnName("IdEnfermedadPlagaMaterial").IsRequired().ValueGeneratedOnAdd();
            builder.Property(epm => epm.MaterialId).HasColumnName("IdMaterial").IsRequired();
            builder.HasOne(epm => epm.Material)
                   .WithMany(m => m.DiseasePlagueMaterialList)
                   .HasForeignKey(epm => epm.MaterialId)
                   .HasPrincipalKey(m => m.Id)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.Property(epm => epm.DiseasePlagueId).HasColumnName("IdEnfermedadPlaga").IsRequired();
            builder.HasOne(epm => epm.DiseasePlague)
                   .WithMany(dp => dp.MaterialDiseasePlagueList)
                   .HasForeignKey(epm => epm.DiseasePlagueId)
                   .HasPrincipalKey(dp => dp.Id)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
