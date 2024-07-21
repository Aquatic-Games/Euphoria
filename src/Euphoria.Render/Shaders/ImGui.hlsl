#pragma vertex VSMain
#pragma pixel PSMain
#pragma debug

#include "Common.hlsli"

struct VSInput
{
    float2 Position: POSITION0;
    float2 TexCoord: TEXCOORD0;
    float4 Color:    COLOR0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
    float4 Color:    COLOR0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer CameraMatrix : register(b0, space0)
{
    float4x4 Projection;
}

EE_SAMPLER2D(Texture, 0, 1);

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

    output.Color = EE_TEXTURE(Texture, input.TexCoord) * input.Color;
    
    return output;
}