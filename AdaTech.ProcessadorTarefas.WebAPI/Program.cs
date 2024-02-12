
using AdaTech.ProcessadorTarefas.Library.Services;
using Microsoft.OpenApi.Models;

namespace AdaTech.ProcessadorTarefas.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddSingleton<ProcessamentoService>();
            builder.Services.AddSingleton<ProcessoService>();
            builder.Services.AddSingleton<TarefaService>();
            builder.Services.AddSingleton<ProcessoExecutorService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProcessadorTarefas API", Version = "v1" });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProcessadorTarefas API v1"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.UseStaticFiles();

            app.UseDefaultFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
