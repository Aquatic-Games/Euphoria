#pragma vertex VSMain
#pragma pixel PSMain
#pragma debug

struct VSInput
{
    float2 Position: POSITION0;
    float4 Color:    COLOR0;
    float2 TexCoord: TEXCOORD0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float4 Color:    COLOR0;
    float2 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer CameraMatrix : register(b0)
{
    float4x4 Projection;
}

Texture2D Texture    : register(t1);
SamplerState Sampler : register(s1);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Projection, float4(input.Position, 0.0, 1.0));
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = Texture.Sample(Sampler, input.TexCoord) * input.Color;
    
    return output;
}