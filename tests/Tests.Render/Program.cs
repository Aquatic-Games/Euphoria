using Euphoria.Render;
using Tests.Render;
using Tests.Render.TestTextureBatcher;
using u4.Math;

Size<int> size = new Size<int>(1280, 720);
GraphicsOptions options = GraphicsOptions.Default;

using TestBase test = new BasicTest();
test.Run(size, options);