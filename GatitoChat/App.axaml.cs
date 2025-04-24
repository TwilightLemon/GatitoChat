using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Net.Http;
using Avalonia.Markup.Xaml;
using GatitoChat.Services;
using GatitoChat.ViewModels;
using GatitoChat.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace GatitoChat;

public partial class App : Application
{
    private static IHost? _host;

    private IHost CreateHost()
        => Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
            .ConfigureServices(RegisterServices)
            .Build();

    /// <summary>
    /// Global HTTP retry policy 
    /// Retries HTTP requests that fail due to transient errors or RequestTimeout
    /// retry up to 3 times, with an exponential backoff strategy
    /// </summary>
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    private void RegisterServices(IServiceCollection services)
    {
        services.AddHttpClient(PublicClientFlag)
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip,
                UseCookies = true,
                UseProxy = true
            }).AddPolicyHandler(RetryPolicy);
        
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        services.AddTransient<LoginWindow>();
        services.AddTransient<LoginWindowViewModel>();

        services.AddTransient<AddRoomWindow>();
        services.AddTransient<AddRoomWindowViewModel>();

        services.AddTransient<AddLocalServerWindow>();
        services.AddTransient<AddLocalServerWindowViewModel>();

        services.AddSingleton<UserProfileService>();
        services.AddSingleton<ChatClientService>();
        services.AddSingleton<LocalChatService>();
    }
    public static T GetRequiredService<T>() where T : notnull => _host!.Services.GetRequiredService<T>();
    public static MainWindow MainWindow => _host!.Services.GetRequiredService<MainWindow>();
    public const string PublicClientFlag = "pcFlag";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Initialize the host and set the main window
        _host = CreateHost();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}