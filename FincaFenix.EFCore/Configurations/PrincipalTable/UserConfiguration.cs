using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.PrincipalTable
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(u => u.User).HasColumnName("NombreUsuario").IsRequired().HasMaxLength(50);
            builder.Property(u => u.Password).HasColumnName("Contrasena").IsRequired().HasMaxLength(100);
            builder.Property(u => u.RoleId).HasColumnName("Rol").IsRequired();
            builder.Property(u => u.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);
        }
    }
}
