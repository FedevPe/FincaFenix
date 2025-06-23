namespace FincaFenix.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplication.CreateBuilder(args)
                .CreateWebApplication()
                .ConfigureApplication()
                .Run();
        }
    }
}
