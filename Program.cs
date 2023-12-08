using GrpcService2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
// Mantap2 yahut
// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();

builder.Services.AddCors(o =>
    o.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders(
                    "Grpc-Status",
                    "Grpc-Message",
                    "Grpc-Encoding",
                    "Grpc-Accept-Encoding");
    }));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Application:Secret").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
        };
    });

var app = builder.Build();
IWebHostEnvironment env = app.Environment;

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.UseCors();

//app.UseAuthentication();

//app.UseAuthorization();

if (env.IsDevelopment())
    app.MapGrpcReflectionService();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<UserService>();
});

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
