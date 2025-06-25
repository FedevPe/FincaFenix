using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.PrincipalTable
{
    public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrderEntity>
    {
        public void Configure(EntityTypeBuilder<WorkOrderEntity> builder)
        {
            builder.ToTable("OrdenTrabajo");
            builder.HasKey(wo => wo.Id);
            builder.Property(wo => wo.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(wo => wo.OrderNum).HasColumnName("NumOrden").IsRequired().HasMaxLength(20);
            builder.Property(wo => wo.UserId).HasColumnName("IdUsuario").IsRequired();
            builder.Property(wo => wo.RecipeId).HasColumnName("IdReceta");
            builder.Property(wo => wo.TaskId).HasColumnName("IdTarea").IsRequired();
            builder.Property(wo => wo.FarmId).HasColumnName("IdFinca").IsRequired();
            builder.Property(wo => wo.StartDate).HasColumnName("FechaInicio").IsRequired().HasColumnType("datetime2(2)");
            builder.Property(wo => wo.EndDate).HasColumnName("FechaFin").HasColumnType("datetime2(2)");
            builder.Property(wo => wo.State).HasColumnName("Estado").IsRequired().HasMaxLength(50);
            builder.Property(wo => wo.Description).HasColumnName("Descripcion").IsRequired(false).HasMaxLength(500);
            builder.Property(wo => wo.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);
            builder.Property(wo => wo.RowVersion).HasColumnName("RowVersion").IsRowVersion().IsRequired().ValueGeneratedOnAddOrUpdate();
        }
    }
}
