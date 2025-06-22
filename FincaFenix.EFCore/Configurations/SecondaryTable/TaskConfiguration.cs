using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.SecondaryTable
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.ToTable("Tarea");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(t => t.Description).HasColumnName("Nombre").IsRequired().HasMaxLength(100);
            builder.Property(t => t.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);  
        }
    }
}
