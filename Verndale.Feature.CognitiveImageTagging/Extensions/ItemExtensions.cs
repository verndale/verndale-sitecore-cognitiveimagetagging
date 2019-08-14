using System.Linq;
using Sitecore;
using Sitecore.Data.Items;

namespace Verndale.Feature.CognitiveImageTagging.Extensions
{
    public static class ItemExtensions
    {
        /// <summary>
        /// Determines if an item is an image by analyzing its template and base templates
        /// </summary>
        /// <param name="item">Sitecore item</param>
        /// <returns>Returns true if image; otherwise, returns false</returns>
        public static bool IsImage(this Item item)
        {
            if (item == null) return false;

            if (item.TemplateID == TemplateIDs.VersionedImage
                || item.TemplateID == TemplateIDs.UnversionedImage)
            {
                return true;
            }

            return item.Template.BaseTemplates.Any(baseTemplate => IsImage(baseTemplate));
        }
    }
}
