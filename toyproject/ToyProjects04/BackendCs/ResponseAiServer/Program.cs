namespace ResponseAiServer {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // 1. HttpClient 등록 - PythonAi 외부서버 등록
            builder.Services.AddHttpClient("PythonAiService", client => {
                client.BaseAddress = new Uri("http://127.0.0.1:8000");
            });

            // 2. CORS 허용(로컬테스트용) -> 실제 운용시 더 정확하게 설정해야 함
            builder.Services.AddCors(options => {
                options.AddDefaultPolicy(policy => {
                    // 개발시 모든 위치의 모든 메서드, 헤더를 허용
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // 3. Controller만 사용하니까
            builder.Services.AddControllers();
            var app = builder.Build();

            // 4. CORS 사용
            app.UseCors();
            
            // 5. index.html 등 일반적 wwwroot 폴더 밑 static 파일 사용
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // 6. Controller 매핑
            app.MapControllers();

            app.Run();
        }
    }
}
