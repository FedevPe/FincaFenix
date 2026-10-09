using FincaFenix.Entities.POCOEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FincaFenix.EFCore.Configurations.Inventory
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<CurrencyEntity>
    {
        public void Configure(EntityTypeBuilder<CurrencyEntity> builder)
        {
            builder.ToTable("Divisa");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("Id").IsRequired().ValueGeneratedOnAdd();
            builder.Property(c => c.Code).HasColumnName("Codigo").IsRequired().HasMaxLength(3);
            builder.Property(c => c.Name).HasColumnName("Nombre").IsRequired().HasMaxLength(50);
            builder.Property(c => c.Symbol).HasColumnName("Simbolo").IsRequired(false).HasMaxLength(10);
            builder.Property(c => c.IsDeleted).HasColumnName("Eliminado").IsRequired().HasDefaultValue(false);

            builder.HasIndex(c => c.Code).IsUnique();

            builder.HasData(
                new CurrencyEntity { Id = 1, Code = "ARS", Name = "Peso Argentino", Symbol = "$", IsDeleted = false },
                new CurrencyEntity { Id = 2, Code = "USD", Name = "Dólar Estadounidense", Symbol = "$", IsDeleted = false },
                new CurrencyEntity { Id = 3, Code = "EUR", Name = "Euro", Symbol = "€", IsDeleted = false },
                new CurrencyEntity { Id = 4, Code = "JPY", Name = "Yen Japonés", Symbol = "¥", IsDeleted = false },
                new CurrencyEntity { Id = 5, Code = "GBP", Name = "Libra Esterlina", Symbol = "£", IsDeleted = false }
            );
        }
    }
}
