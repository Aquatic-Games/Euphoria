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
        //_font = new Font(@"C:\Users\ollie\Downloads\Noto_Sans_SC\static\NotoSansSC-Regular.ttf",
        //    @"C:\Users\ollie\Downloads\Noto_Sans_KR,Noto_Sans_TC\Noto_Sans_KR\static\NotoSansKR-Regular.ttf");
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

        //_font.Draw(batcher, new Vector2(50),
        //    "Hi! This is some text. I like text.\nこのテキストは日本語です。\n我看不懂，但这应该是中文。\n한국어는 어때요?\nИ русский тоже.\n\nAs you can see, it supports \"sub-fonts\",\nso if a character is missing in the main font,\nit will check a sub font.", 50,
        //    Color.White);
        
        Font.Roboto.DrawRichText(batcher, new Vector2(10, 60), "Hello there! <size=50>Hello <size=100>HELLO</size> there. <color=red>This text is red.</color> And this text isn't.\n\n<size=50><color=red>R<color=orange>a<color=yellow>i<color=green>n<color=blue>b<color=indigo>o<color=violet>w</color>!</size>", 30, Color.White);

        const string text = "This should be in the\nbottom right.";
        Size<int> stringSize = Font.Roboto.MeasureString(text, 20, MeasureMode.FullSize);
        Font.Roboto.Draw(batcher, new Vector2(Graphics.Size.Width - stringSize.Width, Graphics.Size.Height - stringSize.Height), text, 20, Color.White);
    }

    public FontTest() : base("Font Test") { }
}