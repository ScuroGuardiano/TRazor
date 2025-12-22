// See https://aka.ms/new-console-template for more information

using BlazorTuiTests.Components;
using BlazorTuiTests.Core;
using Microsoft.AspNetCore.Components;
using Terminal.Gui.App;


IApplication tuiApp = Application.Create();
var builder = Host.CreateApplicationBuilder();
builder.Services.AddSingleton(Dispatcher.CreateDefault());
builder.Services.AddSingleton(tuiApp);
builder.Services.AddSingleton<TuiRenderer>();

var app = builder.Build();

TuiRenderer renderer = app.Services.GetRequiredService<TuiRenderer>();
Dispatcher dispatcher = app.Services.GetRequiredService<Dispatcher>();
tuiApp.Init();

await renderer.StartApplication<App>();

tuiApp.Dispose();
