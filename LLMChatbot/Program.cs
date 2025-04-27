
using LLMChatbot.Models;
using LLMChatbot.Services;
using LLMChatbot.Services.Interfaces;

namespace LLMChatbot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient("LLMClient", client =>
        {
            client.BaseAddress = new Uri("http://localhost:11434/api/");
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        });

        builder.Services.Configure<MongoDbOptions>(
        builder.Configuration.GetSection("MongoDB"));

        builder.Services.AddScoped<ILlmService, LlmService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
