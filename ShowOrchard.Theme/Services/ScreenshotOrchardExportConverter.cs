using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Models;
using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Services;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShowOrchard.Theme.Services;

public class ScreenshotOrchardExportConverter : IOrchardExportConverter
{
    public Task UpdateContentItemsAsync(XDocument document, IList<ContentItem> contentItems)
    {
        var images = document.Root.Element("Content").Elements("Image");
        var oldWebsites = document.Root.Element("Content").Elements("Website");
        var newWebsites = contentItems.Where(item => item.ContentType == "Website");
        var imageIdToMediaUrl = images.ToDictionary(
            key => key.Attribute("Id").Value,
            value => value.Element("MediaPart").Attribute("FolderPath").Value +
                "/" +
                value.Element("MediaPart").Attribute("FileName").Value);

        foreach (var oldWebsite in oldWebsites)
        {
            var newWebsite = newWebsites.First(website =>
                website.As<OrchardIds>().ExportId == oldWebsite.Attribute("Id").Value);
            var screenshotId = oldWebsite.Element("MediaLibraryPickerField.Screenshot").Attribute("ContentItems").Value;
            newWebsite.Content.Website.Screenshot.Paths = JArray.FromObject(new[] { imageIdToMediaUrl[screenshotId] });
        }

        return Task.CompletedTask;
    }
}
