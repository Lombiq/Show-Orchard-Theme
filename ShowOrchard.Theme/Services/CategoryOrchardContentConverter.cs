using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Services;
using OrchardCore.Autoroute.Models;
using OrchardCore.ContentManagement;
using OrchardCore.Taxonomies.Models;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShowOrchard.Theme.Services;

public class CategoryOrchardContentConverter : IOrchardContentConverter
{
    public bool IsApplicable(XElement element) => element.Name == "CategoryTerm";

    public Task ImportAsync(XElement element, ContentItem contentItem)
    {
        contentItem.Alter<TermPart>(part => part.TaxonomyContentItemId = "40z1s1vqgrkzqrbaaw1a913kcr");
        contentItem.Alter<AutoroutePart>(part => part.Absolute = true);

        return Task.CompletedTask;
    }
}
