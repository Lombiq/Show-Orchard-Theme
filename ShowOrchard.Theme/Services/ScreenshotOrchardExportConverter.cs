using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Services;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShowOrchard.Theme.Services;

public class ScreenshotOrchardExportConverter : IOrchardExportConverter
{
    public Task UpdateContentItemsAsync(XDocument document, IList<ContentItem> contentItems)
    {
        var images = document.Root?.Element("Content")?.Elements("Image") ?? [];
        var oldWebsites = document.Root?.Element("Content")?.Elements("Website") ?? [];
        var newWebsites = CategoryOrchardExportConverter.GetNewWebsites(contentItems);

        var imageIdToMediaUrl = images
            .SelectWhere(
                image => new { Id = image.Attribute("Id")?.Value, MediaPart = image.Element("MediaPart") },
                pair => pair.Id != null)
            .ToDictionary(
                pair => pair.Id,
                pair => $"{pair.MediaPart?.Attribute("FolderPath")?.Value}/{pair.MediaPart?.Attribute("FileName")?.Value}");

        foreach (var oldWebsite in oldWebsites)
        {
            var newWebsite = newWebsites.First(website => website.ExportId == oldWebsite.Attribute("Id")?.Value).Website;
            var screenshotId = oldWebsite.Element("MediaLibraryPickerField.Screenshot")?.Attribute("ContentItems")?.Value;
            newWebsite.Content.Website.Screenshot.Paths = JArray.FromObject(new[] { imageIdToMediaUrl[screenshotId] });
        }

        return Task.CompletedTask;
    }
}
