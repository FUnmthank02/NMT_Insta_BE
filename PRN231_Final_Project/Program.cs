using BusinessObjects.Mapping;
using BusinessObjects.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PRN231_Final_Project.Middleware;
using Repositories.Interfaces;
using Repositories.Repositories;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();

builder.Services.AddControllers();

    var credential = GoogleCredential.FromFile("nmt-insta-firebase-adminsdk.json");
    var firebaseApp = FirebaseApp.Create(new AppOptions()
    {
        Credential = credential
    });

    builder.Services.AddSingleton<FirebaseApp>(firebaseApp);




builder.Services.AddTransient<IUserRepository, UserRepository>()
    .AddDbContext<Prn231PrjContext>(opt =>
    builder.Configuration.GetConnectionString("PRN231_PRJ"));

builder.Services.AddTransient<IPostRepository, PostRepository>()
    .AddDbContext<Prn231PrjContext>(opt =>
    builder.Configuration.GetConnectionString("PRN231_PRJ"));

builder.Services.AddTransient<IMediaRepository, MediaRepository>()
    .AddDbContext<Prn231PrjContext>(opt =>
    builder.Configuration.GetConnectionString("PRN231_PRJ"));

builder.Services.AddTransient<ICommentRepository, CommentRepository>()
    .AddDbContext<Prn231PrjContext>(opt =>
    builder.Configuration.GetConnectionString("PRN231_PRJ"));

builder.Services.AddTransient<IReactionRepository, ReactionRepository>()
    .AddDbContext<Prn231PrjContext>(opt =>
    builder.Configuration.GetConnectionString("PRN231_PRJ"));

builder.Services.AddTransient<INotifyRepository, NotifyRepository>()
    .AddDbContext<Prn231PrjContext>(opt =>
    builder.Configuration.GetConnectionString("PRN231_PRJ"));

builder.Services.AddTransient<IFollowRepository, FollowRepository>()
    .AddDbContext<Prn231PrjContext>(opt =>
    builder.Configuration.GetConnectionString("PRN231_PRJ"));

builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>()
    .AddDbContext<Prn231PrjContext>(opt =>
    builder.Configuration.GetConnectionString("PRN231_PRJ"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p => p.AddPolicy("MyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            // Disabling issuer and audience validation
            ValidateIssuer = false,
            ValidateAudience = false,

            // Enabling issuer signing key validation
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyCors");

app.UseHttpsRedirection();

app.UseMiddleware<AuthorizedMiddleware>();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
