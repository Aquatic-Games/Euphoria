using System.Numerics;
using System.Text;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using Euphoria.Render.Text;

namespace Tests.Render.TestTextureBatcher;

public class FontTest : TestBase
{
    private Font _font;
    
    protected override void Initialize()
    {
        _font = new Font(@"C:\Users\ollie\Downloads\Roboto\Roboto-Regular.ttf");
    }

    protected override void Draw()
    {
        TextureBatcher batcher = Graphics.TextureBatcher;

        StringBuilder builder = new StringBuilder();
        for (char c = 'A'; c <= 'z'; c++)
            builder.Append(c);
        
        _font.Draw(batcher, new Vector2(0), builder.ToString(), 50, Color.White);
    }

    public FontTest() : base("Font Test") { }
}