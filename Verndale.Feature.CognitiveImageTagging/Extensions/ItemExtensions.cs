using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Verndale.CognitiveImageTagging;

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

            var compatibleTemplatesString = Sitecore.Configuration.Settings.GetSetting("CompatibleImageTemplateIds");

            if (string.IsNullOrWhiteSpace(compatibleTemplatesString))
            {
                Log.Error("Verndale.CognitiveImageTagging: Unable to load the compatible image template list from Settings.", typeof(ServiceManager));
                return false;
            }

            var compatibleTemplatesArray = compatibleTemplatesString.Split('|');

            foreach (var templateId in compatibleTemplatesArray)
            {
                if (item.TemplateID == new ID(templateId))
                    return true;
            }

            return false;
        }
    }
}
