namespace FincaFenix.Entities.Units
{
    public static class UnitOfMeasureCatalog
    {
        public const int Kilogramo = 1;
        public const int Litro = 2;
        public const int Unidad = 3;
        public const int Metro = 4;
        public const int Bolsa = 5;
        public const int Caja = 6;
        public const int Gramo = 7;
        public const int CentimetroCubico = 8;

        public static readonly IReadOnlyDictionary<int, string> CodeById = new Dictionary<int, string>
        {
            [Kilogramo] = "kg",
            [Litro] = "lts",
            [Unidad] = "unidad",
            [Metro] = "metro",
            [Bolsa] = "bolsa",
            [Caja] = "caja",
            [Gramo] = "gr",
            [CentimetroCubico] = "cc"
        };

        public static readonly IReadOnlyList<(int Id, string Description)> Seed = new List<(int, string)>
        {
            (Kilogramo, "Kilogramo"),
            (Litro, "Litro"),
            (Unidad, "Unidad"),
            (Metro, "Metro"),
            (Bolsa, "Bolsa"),
            (Caja, "Caja"),
            (Gramo, "Gramo"),
            (CentimetroCubico, "Centímetro cúbico")
        };

        public static int? GetIdByCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            var normalized = code.Trim().ToLowerInvariant();

            foreach (var pair in CodeById)
            {
                if (pair.Value == normalized)
                {
                    return pair.Key;
                }
            }

            return null;
        }
    }
}
