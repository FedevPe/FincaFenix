using FincaFenix.Entities.Exceptions;

namespace FincaFenix.Entities.Units
{
    public static class UnitConverter
    {
        private enum UnitFamily
        {
            Mass,
            Volume,
            Count,
            Length,
            Package
        }

        private static readonly Dictionary<string, (UnitFamily Family, decimal FactorToBase)> Units = new()
        {
            ["kg"] = (UnitFamily.Mass, 1m),
            ["gr"] = (UnitFamily.Mass, 0.001m),
            ["lts"] = (UnitFamily.Volume, 1m),
            ["cc"] = (UnitFamily.Volume, 0.001m),
            ["unidad"] = (UnitFamily.Count, 1m),
            ["metro"] = (UnitFamily.Length, 1m),
            ["bolsa"] = (UnitFamily.Package, 1m),
            ["caja"] = (UnitFamily.Package, 1m)
        };

        public static decimal ConvertToBaseUnit(decimal amount, string? sourceUnit, int? baseUnitId)
        {
            if (!baseUnitId.HasValue)
            {
                return amount;
            }

            if (!UnitOfMeasureCatalog.CodeById.TryGetValue(baseUnitId.Value, out var baseCode))
            {
                return amount;
            }

            if (string.IsNullOrWhiteSpace(sourceUnit))
            {
                return amount;
            }

            var sourceCode = Normalize(sourceUnit);

            if (sourceCode is null)
            {
                throw new BusinessRuleException($"Unidad de medida desconocida: '{sourceUnit}'.");
            }

            if (sourceCode == baseCode)
            {
                return amount;
            }

            var from = Units[sourceCode];
            var to = Units[baseCode];

            if (from.Family != to.Family)
            {
                throw new BusinessRuleException(
                    $"No se puede convertir '{sourceUnit}' a '{baseCode}': las unidades son incompatibles.");
            }

            return amount * from.FactorToBase / to.FactorToBase;
        }

        public static string? Normalize(string? unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
            {
                return null;
            }

            return unit.Trim().ToLowerInvariant() switch
            {
                "kg" or "kgs" or "kilo" or "kilos" or "kilogramo" or "kilogramos" => "kg",
                "gr" or "grs" or "gramo" or "gramos" => "gr",
                "lts" or "lt" or "l" or "litro" or "litros" => "lts",
                "cc" or "ml" or "cm3" or "centimetrocubico" or "centímetrocubico" => "cc",
                "unidad" or "unidades" or "un" or "u" or "uni" => "unidad",
                "metro" or "metros" or "m" => "metro",
                "bolsa" or "bolsas" => "bolsa",
                "caja" or "cajas" => "caja",
                _ => null
            };
        }
    }
}
