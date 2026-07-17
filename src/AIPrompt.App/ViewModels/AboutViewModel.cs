using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommunityToolkit.Mvvm.Input;

namespace AIPrompt.App.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    public string ApplicationName => "AIPrompt";

    public string VersionText => $"Version {Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.0.0"}";

    public string CopyrightText => "Copyright © 2026 SANDEFJORD DEVELOPMENT — All rights reserved";

    public string CreatorName => "Patrick JAILLET";

    public string Email => "contact.shaderstudio@gmail.com";

    public string WebsiteUrl => "https://github.com/Patrickjaillet";

    public string RepositoryUrl => "https://github.com/Patrickjaillet/AIPrompt";

    public string LicenseText => "License: MIT";

    [RelayCommand]
    private void OpenEmail() => OpenUrl($"mailto:{Email}");

    [RelayCommand]
    private void OpenWebsite() => OpenUrl(WebsiteUrl);

    [RelayCommand]
    private void OpenRepository() => OpenUrl(RepositoryUrl);

    [RelayCommand]
    private void OpenLicense()
    {
        var licensePath = Path.Combine(AppContext.BaseDirectory, "LICENSE.txt");
        OpenUrl(licensePath);
    }

    private static void OpenUrl(string target)
    {
        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
    }
}
