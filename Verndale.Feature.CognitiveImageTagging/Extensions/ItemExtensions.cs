using Constellation.Foundation.Data;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Exceptions;
using Verndale.CognitiveImageTagging;
using Verndale.CognitiveImageTagging.ImageTaggers;

namespace Verndale.Feature.CognitiveImageTagging.Extensions
{
	/// <summary>
	/// The Methods used to contact the Cognitive Services image tagger. These are marked internal to prevent
	/// misuse outside of this Feature module.
	/// </summary>
	internal static class ItemExtensions
	{
		/// <summary>
		/// Determines if an item is an image by checking for inheritance from the base Image templates.
		/// </summary>
		/// <param name="mediaItem">Sitecore item</param>
		/// <returns>Returns true if image; otherwise, returns false</returns>
		internal static bool IsImage(this Item item, out MediaItem mediaItem)
		{
			mediaItem = null;

			if (item == null) return false;


			if (item.IsDerivedFrom(TemplateIDs.UnversionedImage) || item.IsDerivedFrom(TemplateIDs.VersionedImage))
			{
				mediaItem = new MediaItem(item);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Uses Cognitive Services to analyze the supplied Image and return a descriptive caption, a series of tags, and/or any text discovered on
		/// the image.
		/// </summary>
		/// <param name="mediaItem">The Image item to analyze.</param>
		/// <returns>One of the following based on confidence: a descriptive caption, a series of tag words, any OCR text in the image, or null.</returns>
		// ReSharper disable once InconsistentNaming
		internal static string GetAltTextFromAI(this MediaItem mediaItem)
		{
			var imageStream = mediaItem.GetMediaStream();

			var connectionName = Sitecore.Configuration.Settings.GetSetting("CognitiveImageTagging.ConnectionName");
			IImageTagger tagger = null;

			try
			{
				tagger = ImageTaggingManager.GetImageTagger(connectionName);
			}
			catch (ConfigurationException ex)
			{
				Log.Error("CognitiveImageTagging: Could not load an image tagger due to a configuration error. Check that you have a valid connection specified.", ex, typeof(IImageTagger));
				throw;
			}

			var result = tagger.GetImageDescription(imageStream, mediaItem.InnerItem.Language.Name, true);

			switch (result.Status)
			{
				case ImageResult.ResultStatus.Error:
					Log.Error("Verndale.Feature.CognitiveImageTagging encountered an error getting Alt text from the AI service.", result.Exception, result);
					return null;
				case ImageResult.ResultStatus.NoResponse:
					Log.Warn("Verndale.Feature.CognitiveImageTagging: Received no valid response from AI service.", result);
					return null;
			}

			if (result.Captions != null)
			{
				foreach (var caption in result.Captions)
				{
					return caption; // we only need the first one.
				}
			}

			Log.Warn("CognitivieImageTagging: No captions were returned with sufficient confidence to be used as ALT text. If this happens frequently, consider lowering your confidence level.", typeof(IImageTagger));

			// We didn't have any captions! Concatenate any tags just to get the content author started.
			if (result.Tags != null)
			{
				return string.Join(", ", result.Tags);
			}

			Log.Debug("Verndale.Feature.CognitiveImageTagging: result from AI service did not contain any captions or tags to use as Alt text.", result);
			return null;
		}
	}
}
