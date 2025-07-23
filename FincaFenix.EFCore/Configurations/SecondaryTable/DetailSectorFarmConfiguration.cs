using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class DetailSectorFarmConfiguration : IEntityTypeConfiguration<DetailSectorFarmEntity>
    {
        public void Configure(EntityTypeBuilder<DetailSectorFarmEntity> builder)
        {
            builder.ToTable("DetalleSectorFinca");
            builder.HasKey(dsf => dsf.Id);
            builder.Property(dsf => dsf.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(dsf => dsf.FarmId).HasColumnName("IdFinca").IsRequired();
            builder.Property(dsf => dsf.SectorName).HasColumnName("NombreCuadro").IsRequired();
            builder.Property(dsf => dsf.VarietyId).HasColumnName("IdVariedad");
            builder.Property(dsf => dsf.NumberPlants).HasColumnName("CantPlantas");
            builder.Property(dsf => dsf.Area).HasColumnName("Hectareas").HasColumnType("decimal(5,2)");
        }
    }
}
