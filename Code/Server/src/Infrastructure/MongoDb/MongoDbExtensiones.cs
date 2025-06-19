namespace IMT_Reservas.Server.Infrastructure.MongoDb
{
    public static class MongoDbExtensiones
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbConfiguracion>(configuration.GetSection("MongoDbSettings"));
            services.AddSingleton<MongoDbContexto>();

            return services;
        }
    }
}
