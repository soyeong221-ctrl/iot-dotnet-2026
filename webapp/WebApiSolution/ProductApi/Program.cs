
namespace ProductApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connString = builder.Configuration.GetConnectionString("TestDbConnection");

            Console.WriteLine(connString);

            //builder.Services.AddDbVontext<> EntityFramework를 안 쓰기 때문에 사용 불가

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // CORS 설정 - 외부서버 접근 허용
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.UseCors("AllowAll");    // CORS 사용

            app.Run();
        }
    }
}
