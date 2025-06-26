using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class CorrelativeNumberConfiguration : IEntityTypeConfiguration<CorrelativeNumberEntity>
    {
        public void Configure(EntityTypeBuilder<CorrelativeNumberEntity> builder)
        {
            builder.ToTable("Correlativo");
            builder.HasKey("Id");
            builder.Property(cn => cn.TypeDoc).IsRequired().HasColumnName("TipoDoc").HasMaxLength(50);
            builder.Property(cn => cn.LastNumber).IsRequired().HasColumnName("Numero");
        }
    }
}
