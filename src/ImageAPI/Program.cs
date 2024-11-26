using ImageAPI.DAL;
using Serilog;
using Azure.Storage.Blobs;
using FluentValidation;
using ImageAPI.Contracts;
using ImageAPI.Validators;
using Microsoft.AspNetCore.Mvc;

namespace ImageAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
              .AddJsonFile("appsettings.json")
              .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json")
              .AddEnvironmentVariables();

            builder.Services.Configure<AzureBlobSettings>(
               builder.Configuration.GetSection("AzureBlobSettings"));
                        
            builder.Services.AddControllers();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
    
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(["http://localhost:8080", "http://localhost:8081"])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            builder.Services.AddSwaggerGen();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var blobSettings = builder.Configuration.GetSection("AzureBlobSettings").Get<AzureBlobSettings>();

            builder.Services.AddSingleton(x => new BlobServiceClient(blobSettings.AzureBlobConnectionString));

            builder.Services.AddSingleton<Serilog.ILogger>(logger);
            builder.Services.AddSingleton<IImagesRepository, AzureBlobImagesRepository>();
            builder.Services.AddSingleton<IValidator<ImageUploadRequest>, ImageUploadRequestValidator>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowSpecificOrigin");
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
