using Euphoria.Render;
using grabs.Graphics;
using Tests.Render;
using Tests.Render.Render3D;
using Tests.Render.TestTextureBatcher;
using u4.Core;
using u4.Math;

Logger.AttachConsole();

Size<int> size = new Size<int>(1280, 720);
GraphicsOptions options = GraphicsOptions.Default;

using TestBase test = new PlaneTest();
test.Run(size, GraphicsApi.D3D11, options);