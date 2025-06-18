namespace IMT_Reservas.Server.Infrastructure.MongoDb
{
    public static class MongoDbExtensiones
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbConfiguracion>(configuration.GetSection("MongoDbConfiguracion"));
            
            services.AddSingleton<MongoDbContexto>();

            return services;
        }
    }
}

