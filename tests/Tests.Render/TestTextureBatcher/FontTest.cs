using System;
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
        _font = new Font(@"C:\Users\ollie\Documents\SpaceBox\Old\Fonts\daggersquare.otf", @"C:\Users\ollie\Downloads\Noto_Sans_JP\static\NotoSansJP-Regular.ttf", @"C:\Users\ollie\Downloads\Noto_Sans_KR,Noto_Sans_TC\Noto_Sans_KR\static\NotoSansKR-Regular.ttf");
    }

    protected override void Draw()
    {
        TextureBatcher batcher = Graphics.TextureBatcher;

        StringBuilder builder = new StringBuilder();
        /*for (char c = '\0'; c <= 1000; c++)
        {
            if (c % 40 == 0)
                builder.Append('\n');
            
            builder.Append(c);
        }

        _font.Draw(batcher, new Vector2(0), builder.ToString(), 50, Color.White);*/

        _font.Draw(batcher, new Vector2(50),
            "Hi! This is some text. I like text.\nこのテキストは日本語です。\n한국어는 어때요?\nИ русский тоже.\n\nAs you can see, it supports \"sub-fonts\",\nso if a character is missing in the main font,\nit will check a sub font.", 50,
            Color.White);
    }

    public FontTest() : base("Font Test") { }
}