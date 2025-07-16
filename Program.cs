using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using GrpcProductClient.Components;
using GrpcProductService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton(sp =>
{
    Console.WriteLine("Creating gRPC channel...");
    // allow plaintext HTTP/2
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

    var channel = GrpcChannel.ForAddress("http://localhost:5157",
        new GrpcChannelOptions
        {
            MaxReceiveMessageSize = 100 * 1024 * 1024   // optional, large messages
        });

    Console.WriteLine("Channel created. Creating client...");
    return new Product.ProductClient(channel);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
