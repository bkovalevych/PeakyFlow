using Microsoft.AspNetCore.Components;

namespace PeakyFlow.Aspire.Mobile.Services;

public class NavigationService
{
    private readonly NavigationManager _navigationManager;
    private readonly string _basePath;

    public NavigationService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        
        // Determine base path based on environment
        var uri = new Uri(_navigationManager.BaseUri);
        _basePath = uri.Host.Contains("github.io") ? "/PeakyFlow" : "";
    }

    public string GetBasePath() => _basePath;

    public void NavigateTo(string relativePath)
    {
        var fullPath = $"{_basePath}{relativePath}";
        _navigationManager.NavigateTo(fullPath);
    }
}
