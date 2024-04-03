using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Models;
using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Services;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
using System.Collections.Generic;
using System.Linq;
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
        var categoryPart = category.As<TaxonomyPart>();
        var importedTerms = contentItems.Where(item => item.ContentType == "CategoryTerm").ToList();
        var existingTerms = new List<ContentItem>();
        foreach (var importedTerm in importedTerms)
        {
            var existingTerm = categoryPart.Terms
                .Find(term => term.As<OrchardIds>()?.ExportId == importedTerm.As<OrchardIds>().ExportId);
            if (existingTerm != null)
            {
                importedTerm.ContentItemId = existingTerm.ContentItemId;
                existingTerms.Add(importedTerm);
            }
        }

        var aliasToTermIds = importedTerms.ToDictionary(key => key.As<OrchardIds>().ExportId, value => value.ContentItemId);
        var oldWebsites = document.Root.Element("Content").Elements("Website");
        var newWebsites = contentItems.Where(item => item.ContentType == "Website");
        foreach (var oldWebsite in oldWebsites)
        {
            var oldTermIds = oldWebsite.Element("TaxonomyField.Category").Attribute("Terms").Value.Split(',');
            var newWebsite = newWebsites.First(website =>
                website.As<OrchardIds>().ExportId == oldWebsite.Attribute("Id").Value);
            newWebsite.Content.Website.Category.TaxonomyContentItemId = category.ContentItemId;
            newWebsite.Content.Website.Category.TermContentItemIds = JArray.FromObject(oldTermIds.Select(id => aliasToTermIds[id]).ToArray());
        }

        importedTerms.RemoveAll(existingTerms.Contains);
        if (importedTerms.Count != 0)
        {
            category.Alter<TaxonomyPart>(part => part.Terms.AddRange(importedTerms));
            contentItems.Add(category);
        }

        contentItems.RemoveAll(item => item.ContentType == "CategoryTerm");
    }
}
