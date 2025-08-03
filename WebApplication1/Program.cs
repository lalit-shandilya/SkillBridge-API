using Azure;
using Azure.AI.DocumentIntelligence;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Win32;
using SB.API;
using SB.Application;
using SB.Application.Commands;
using SB.Application.Features.Profile.Commands;
using SB.Application.Features.Users.Commands;
using SB.Application.Queries;
using SB.Application.Services.Implementation;
using SB.Application.Services.Interface;
using SB.Domain;
using SB.Infrastructure.Persistence;
using SB.Infrastructure.Repositories.Implementation;
using SB.Infrastructure.Repositories.Interfaces;
using SB.Infrastructure.Services;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);



// Register CosmosClient FIRST
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var endpoint =  configuration["CosmosDb:AccountEndpoint"];
    var key = configuration["CosmosDb:AccountKey"];

    if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
    {
        throw new ArgumentNullException("Cosmos DB configuration is missing.");
    }

    return new CosmosClient(endpoint, key);
});

// Inject CosmosClient into DbContext
builder.Services.AddSingleton<CosmosDbContext>(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    var databaseName = configuration["CosmosDb:DatabaseName"];

    return new CosmosDbContext(cosmosClient, databaseName);
});

builder.Services.AddSingleton<ProfileDbContext>(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    var databaseName = configuration["CosmosDb:DatabaseName"];

    return new ProfileDbContext(cosmosClient, databaseName);
});

builder.Services.AddSingleton<DocumentIntelligenceClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var endpoint = configuration["DocumentIntelligence:Endpoint"];
    var apiKey = configuration["DocumentIntelligence:ApiKey"];

    var credential = new AzureKeyCredential(apiKey);
    var client = new DocumentIntelligenceClient(new Uri(endpoint), credential);
    return client;
});


// Add Application Services
//builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton<IUserService<EmployeeUser>>(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    return new UserService<EmployeeUser>(cosmosClient, "SB_database", "SB_Container");
});

builder.Services.AddSingleton<IUserService<EmployerUser>>(sp =>
{
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    return new UserService<EmployerUser>(cosmosClient, "SB_database", "SB_Container");
});


builder.Services.AddScoped(typeof(IUserRepository<>), typeof(UserRepository<>));
// Register Azure AI Search dependencies
builder.Services.AddSingleton<JobSearchIndexService>();
builder.Services.AddSingleton<JobSearchService>();

builder.Services.AddSingleton<IJobSearchRepository, JobSearchRepository>();
builder.Services.AddSingleton<IUserProfileRepository, UserProfileRepository>();

builder.Services.AddMediatR(config =>
{
    // Register handlers in the current assembly
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly);
    config.RegisterServicesFromAssembly(typeof(UploadResumeCommandHandler).Assembly); 
    config.RegisterServicesFromAssembly(typeof(SearchJobsQueryHandler).Assembly); 
    config.RegisterServicesFromAssembly(typeof(CreateJobPostingHandler).Assembly); 
    config.RegisterServicesFromAssembly(typeof(SearchJobsBySkillsQueryHandler).Assembly);
    config.RegisterServicesFromAssemblies(typeof(UploadResumeHandler).Assembly);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});


// Add Controllers
builder.Services.AddControllers();
//builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
    c.OperationFilter<FileUploadOperationFilter>();
});


// Add configuration for AzureOpenAISettings
builder.Services.Configure<SB.Application.Features.Profile.Commands.OpenAI>(builder.Configuration.GetSection("OpenAI"));

// Register repository
builder.Services.AddScoped<IJobPostingRepository, JobPostingRepository>();
// Register IHttpClientFactory
builder.Services.AddHttpClient();

// Register UploadResumeCommandHandler
//builder.Services.AddScoped<SB.Application.Features.Profile.Commands.UploadResumeCommandHandler>();
// Access the configuration
IConfiguration configuration = builder.Configuration;
string blobConnectionString = configuration["ConnectionStringsBlob:AzureBlobStorage"];
// Load settings from configuration
builder.Services.Configure<AzureCognitiveSearch>(configuration.GetSection("AzureCognitiveSearch"));
builder.Services.Configure<DocumentIntelligence>(configuration.GetSection("DocumentIntelligence"));
builder.Services.Configure<CosmosDb>(configuration.GetSection("CosmosDb"));
// Register BlobServiceClient with DI
builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));

// Register MediatR
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

// Register BlobStorageService
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();



//builder.Services.Configure<AzureSearchSettings>(
//    builder.Configuration.GetSection("AzureSearch"));

// Register MediatR (Ensure Application assembly is included)
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SearchJobsQueryHandler).Assembly));
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateJobPostingHandler).Assembly));



var app = builder.Build();
builder.Logging.AddConsole();

// This ensures the app listens to the right port (usually from environment variable)
var port ="8080";
app.Urls.Add($"http://*:{port}");

//if (app.Environment.IsDevelopment())
//{
app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArchitecture API v1"));
//}

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddGoogle(options =>
//{
//    options.ClientId = "Your-Client-ID";
//    options.ClientSecret = "Your-Client-Secret";
//});
app.UseCors();
app.MapGet("/health", () => Results.Ok("Healthy"));
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
