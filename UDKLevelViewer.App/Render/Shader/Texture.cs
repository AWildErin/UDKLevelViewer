using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using LegendaryExplorerCore.Gammtek.Extensions;
using LegendaryExplorerCore.Textures;
using LegendaryExplorerCore.Unreal.Classes;
using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using Image = SixLabors.ImageSharp.Image;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace UDKLevelViewer.App.Render
{
    public class Texture
    {
        protected int Handle { get; }

        private Texture(int glHandle)
        {
            Handle = glHandle;
        }

        public static Texture LoadFromFile(string path, bool generateMipmaps = true) => new(CreateTexture(Image.Load<Rgba32>(path), generateMipmaps));

        public static Texture LoadFromImage(Image<Rgba32> image, bool generateMipmaps = true) => new(CreateTexture(image, generateMipmaps));

        // TODO: Errors when tfc can't be found or is malformed.
        public static Texture LoadFromTexture2D(Texture2D texture)
		{
            var format = texture.TextureFormat;
            var topMip = texture.GetTopMip();

            Image<Rgba32> image;
            if (topMip == null)
                image = Image.Load<Rgba32>("Data/textures/Unreal/defaultdiffuse.png");
            else
			{
                var imageBytes = Texture2D.GetTextureData(topMip, topMip.Export.Game);
                var test = LegendaryExplorerCore.Textures.Image.convertRawToBitmapARGB(imageBytes, topMip.width, topMip.height, LegendaryExplorerCore.Textures.Image.getPixelFormatType(format));
                var memory = new MemoryStream(test.Height * test.Width * 4 + 54);
                test.Save(memory, ImageFormat.Bmp);

                image = Image.Load<Rgba32>(memory.ToArray());
            }

            return new Texture(CreateTexture(image, true));
		}

        private static int CreateTexture(Image<Rgba32> image, bool generateMipmaps)
        {
            int handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
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
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());
            if (generateMipmaps)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                    (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                    (int)TextureWrapMode.Repeat);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                    (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                    (int)TextureWrapMode.Repeat);
            }
            return handle;
        }

        public void Use(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}