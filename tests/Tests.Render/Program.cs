using Euphoria.Core;
using Euphoria.Math;
using Euphoria.Render;
using grabs.Graphics;
using Tests.Render;
using Tests.Render.Render3D;
using Tests.Render.TestTextureBatcher;

Logger.AttachConsole();

Size<int> size = new Size<int>(1280, 720);
GraphicsSettings settings = GraphicsSettings.Default;

using TestBase test = new PbrTest();
test.Run(size, GraphicsApi.D3D11, settings);