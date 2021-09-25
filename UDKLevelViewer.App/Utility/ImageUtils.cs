using OpenTK.Windowing.Common.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using Image = SixLabors.ImageSharp.Image;

namespace UDKLevelViewer.App.Utility
{
    public class ImageUtils
    {
        
		/// <summary>
		/// Takes a ImageSharp image and returns an OpenTK image.
		/// </summary>
		/// <param name="image"></param>
		/// <returns>OpenTK Image</returns>
        public static OpenTK.Windowing.Common.Input.Image LoadImage(Image<Rgba32> image)
		{
			var pixels = new List<byte>(4 * image.Width * image.Height);
			for (var y = 0; y < image.Height; y++)
			{
				var row = image.GetPixelRowSpan(y);
				for (var x = 0; x < image.Width; x++)
				{
					pixels.Add(row[x].R);
					pixels.Add(row[x].G);
					pixels.Add(row[x].B);
					pixels.Add(row[x].A);
				}
			}
			return new OpenTK.Windowing.Common.Input.Image(image.Height, image.Width, pixels.ToArray());
		}

    }
}