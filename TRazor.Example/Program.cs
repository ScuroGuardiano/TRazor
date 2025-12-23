using TRazor;
using TRazor.Example.Components;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTRazor<App>();

var app = builder.Build();

app.Run();