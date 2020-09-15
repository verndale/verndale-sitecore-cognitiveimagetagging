using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.Upload;
using Sitecore.SecurityModel;
using Verndale.Feature.CognitiveImageTagging.Extensions;

namespace Verndale.Feature.CognitiveImageTagging.Pipelines.Upload
{
	public class AddImageAltTextDuringUpload
	{
		/// <summary>
		/// Uses Verndale.CognitiveImageTagging services to analyze the image and provide possible captions.
		/// Assigns the first caption to the Alt field in the Image section upon execution of the command.
		/// Reference should be placed above "Done" and below "Save" in uiUpload Processor
		/// </summary>
		public void Process(UploadArgs args)
		{
			Assert.ArgumentNotNull((object)args, nameof(args));

			var list = args.UploadedItems;

			Log.Info($"Verndale.Feature.CognitiveImageTagging: Upload Pipeline: processing AI Image Tags for {list.Count} images", this);

			foreach (var item in list)
			{
				if (item == null || !item.IsImage(out var mediaItem)) continue;

				var text = mediaItem.GetAltTextFromAI();

				using (new SecurityDisabler())
				{

					using (new EditContext(mediaItem))
					{
						mediaItem.InnerItem.Fields["Alt"].Value = text;
					}

				}
			}

			Log.Info($"Verndale.CognitiveImageTagging: Ending AddImageAltText in uiUpload processor", this);
		}
	}
}
