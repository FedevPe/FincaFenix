using FincaFenix.PDF;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddPDFServices(this IServiceCollection services)
        {
            services.AddTransient<IPdfGenerationService, WorkOrderPdfService>();

            return services;
        }
    }
}
