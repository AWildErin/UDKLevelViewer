using System;
using System.Collections.Generic;
using System.Diagnostics;
using LegendaryExplorerCore.Unreal;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using UDKLevelViewer.App.Render;
using UDKLevelViewer.App.Core;
using OpenTK.Windowing.Common.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;
using UDKLevelViewer.App.Utility;

namespace UDKLevelViewer.App
{
	public static class Program
	{
		private static void Main(string[] args)
		{
			var nativeWindowSettings = new NativeWindowSettings()
			{
				Size = new Vector2i(800, 600),
				Title = "UDK Level Viewer",
				Icon = new WindowIcon(ImageUtils.LoadImage(Image.Load<Rgba32>("data/textures/unreal/defaultdiffuse.png")))
			};

			using (var window = new MainWindow(GameWindowSettings.Default, nativeWindowSettings))
			{
				window.Run();
			}
		}
	}
}
