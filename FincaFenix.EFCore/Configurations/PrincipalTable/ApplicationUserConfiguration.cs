using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.PrincipalTable
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.IsActive).HasColumnName("IsActive").IsRequired().HasDefaultValue(true);
        }
    }
}