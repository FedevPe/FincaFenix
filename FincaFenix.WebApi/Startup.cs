namespace UniversitarySistem.WebApi
{
    internal static class Startup
    {
        public static WebApplication CreateWebApplication (
            this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //builder.Services.AddUniversitarySistemServices(dbOptions => 
            //    builder.Configuration.GetSection(DBOption.SectionKey).Bind(dbOptions));
            //builder.Services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(config =>
            //    {
            //        config.AllowAnyMethod();
            //        config.AllowAnyHeader();
            //        config.AllowAnyOrigin();
            //    });
            //});

            return builder.Build();
        }
        public static WebApplication ConfigureWebApplication
            (this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            app.UseRouting();
            app.MapControllers();

            return app;
        }
    }
}
