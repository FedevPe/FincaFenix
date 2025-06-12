namespace UniversitarySistem.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplication.CreateBuilder(args)
                .CreateWebApplication()
                .ConfigureWebApplication()
                .Run();
        }
    }
}
