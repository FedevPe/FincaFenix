namespace FincaFenix.EFCore.Options
{
    public class DBOption
    {
        public const string SectionKey = nameof(DBOption);
        public string ConnectionString { get; set; }
    }
}
