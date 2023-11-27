using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Services;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShowOrchard.Theme.Services;

public class CreatedUtcAdjustingOrchardExportConverter : IOrchardExportConverter
{
    private readonly IContentManager _contentManager;

    public CreatedUtcAdjustingOrchardExportConverter(IContentManager contentManager) =>
        _contentManager = contentManager;

    public Task UpdateContentItemsAsync(XDocument document, IList<ContentItem> contentItems)
    {
        var contentItemsWithDuplicateCreatedUtc = contentItems
            .GroupBy(contentItem => contentItem.CreatedUtc)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group)
            .Reverse()
            .ToList();

        // Add a second to the CreatedUtc of the duplicate content items.
        foreach (var contentItem in contentItemsWithDuplicateCreatedUtc)
        {
            var numberOfSecondsToAdd = contentItemsWithDuplicateCreatedUtc
                .Count(item => item.CreatedUtc == contentItem.CreatedUtc) - 1;
            contentItem.CreatedUtc = contentItem.CreatedUtc?.AddSeconds(numberOfSecondsToAdd);
        }

        return Task.CompletedTask;
    }
}
