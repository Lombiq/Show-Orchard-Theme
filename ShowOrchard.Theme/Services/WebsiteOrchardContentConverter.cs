using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Services;
using OrchardCore.ContentManagement;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShowOrchard.Theme.Services;

public class WebsiteOrchardContentConverter : IOrchardContentConverter
{
    public bool IsApplicable(XElement element) => element.Name == "Website";

    public Task ImportAsync(XElement element, ContentItem contentItem)
    {
        var developerField = element.Element("LinkField.Developer");
        if (!string.IsNullOrEmpty(developerField?.Attribute("Url")?.Value))
        {
            contentItem.Content.Website.Developer.Url = developerField.Attribute("Url")?.Value;
            contentItem.Content.Website.Developer.Text = developerField.Attribute("Text")?.Value;
        }

        var websiteField = element.Element("LinkField.Website");
        if (!string.IsNullOrEmpty(websiteField?.Attribute("Url")?.Value))
        {
            contentItem.Content.Website.Website.Url = websiteField.Attribute("Url")?.Value;
            contentItem.Content.Website.Website.Text = websiteField.Attribute("Text")?.Value;
        }

        contentItem.Content.Website.Interview.Value =
            bool.Parse(element.Element("BooleanField.Interview")?.Attribute("Value")?.Value);

        return Task.CompletedTask;
    }
}
