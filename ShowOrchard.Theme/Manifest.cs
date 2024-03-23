using Lombiq.Hosting.MediaTheme.Bridge.Constants;
using OrchardCore.DisplayManagement.Manifest;

[assembly: Theme(
    Name = "ShowOrchard.Theme",
    Author = "The Orchard Core Team",
    Website = "https://orchardcore.net",
    Version = "0.0.1",
    Description = "ShowOrchard.Theme",
    BaseTheme = "Lombiq.BaseTheme",
    Dependencies =
    [
        FeatureNames.MediaThemeBridge,
    ]
)]
