using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Quartz;
bool production = bool.Parse(AppSettings.GetSetting("production"));
string routePrefix = AppSettings.GetSetting("ruotePrefix");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", builder =>
  {
    builder.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader();
  });
});
//Job
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    ConfiguracionCronJob.RegistrarTrabajo<WorkerSendSms>(
        quartzConfigurator: q,
        jobKey: "WorkerSendSms",
        triggerKey: "WorkerSendSms-trigger",
        cronExpression: AppSettings.GetSetting("tiempojob:envioSms") // Ejecutar cada minuto
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Configurar la autenticación
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API V1", Version = "v1" });

    // Definir esquema de seguridad Basic
    options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Ingrese su nombre de usuario y contraseña para autenticarse"
    });

    // Aplicar esquema a todos los endpoints (opcional: puedes aplicarlo a uno si quieres)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHostedService<WorkerService>();


var app = builder.Build();

// Habilitar CORS con la política "AllowAll"
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Excluir Swagger de la autenticación
app.UseSwagger();

app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint($"{(production ? "/" + routePrefix : "")}/swagger/v1/swagger.json", "API V1");
  c.RoutePrefix = "swagger"; // Ruta de Swagger
});

// Configura la autenticación y autorización para todo el pipeline
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();
