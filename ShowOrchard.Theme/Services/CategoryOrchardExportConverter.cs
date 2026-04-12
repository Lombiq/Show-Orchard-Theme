using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Models;
using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Services;
using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShowOrchard.Theme.Services;

public class CategoryOrchardExportConverter : IOrchardExportConverter
{
    private readonly IContentManager _contentManager;

    public CategoryOrchardExportConverter(IContentManager contentManager) =>
        _contentManager = contentManager;

    public async Task UpdateContentItemsAsync(XDocument document, IList<ContentItem> contentItems)
    {
        var category = await _contentManager.GetAsync("40z1s1vqgrkzqrbaaw1a913kcr");
        var categoryPart = category.GetOrCreate<TaxonomyPart>();
        var importedTerms = contentItems.Where(item => item.ContentType == "CategoryTerm").ToList();
        var existingTerms = new List<ContentItem>();
        foreach (var importedTerm in importedTerms)
        {
            var existingTerm = categoryPart.Terms
                .Find(term => term.GetMaybe<OrchardIds>()?.ExportId == importedTerm.GetMaybe<OrchardIds>()?.ExportId);
            if (existingTerm != null)
            {
                importedTerm.ContentItemId = existingTerm.ContentItemId;
                existingTerms.Add(importedTerm);
            }
        }

        var aliasToTermIds = importedTerms.ToDictionary(key => key.GetOrCreate<OrchardIds>().ExportId, value => value.ContentItemId);
        var oldWebsites = document.Root?.Element("Content")?.Elements("Website").CastWhere<XElement>() ?? [];
        var newWebsites = GetNewWebsites(contentItems);

        foreach (var oldWebsite in oldWebsites)
        {
            var oldTermIds = oldWebsite.Element("TaxonomyField.Category")?.Attribute("Terms")?.Value.Split(',') ?? [];
            if (newWebsites.First(pair => pair.ExportId == oldWebsite.Attribute("Id")?.Value).Website is { } newWebsite)
            {
                newWebsite.Content.Website.Category.TaxonomyContentItemId = category.ContentItemId;
                newWebsite.Content.Website.Category.TermContentItemIds =
                    JArray.FromObject(oldTermIds.Select(id => aliasToTermIds[id]).ToArray());
            }
        }

        importedTerms.RemoveAll(existingTerms.Contains);
        if (importedTerms.Count != 0)
        {
            category.Alter<TaxonomyPart>(part => part.Terms.AddRange(importedTerms));
            contentItems.Add(category);
        }

        contentItems.RemoveAll(item => item.ContentType == "CategoryTerm");
    }

    internal static List<(ContentItem Website, string ExportId)> GetNewWebsites(IList<ContentItem> contentItems) =>
        contentItems
            .SelectWhere(
                item => (Website: item, item.GetMaybe<OrchardIds>()?.ExportId),
                pair => pair.Website.ContentType == "Website" && !string.IsNullOrEmpty(pair.ExportId))
            .ToList();
}
