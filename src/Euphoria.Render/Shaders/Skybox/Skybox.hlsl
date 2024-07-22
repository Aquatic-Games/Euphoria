#pragma vertex VSMain
#pragma pixel PSMain
#pragma debug

#include "../Common.hlsli"

struct VSInput
{
    float3 Position: POSITION0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float3 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer CameraMatrices : register(b0, space0)
{
    float4x4 Projection;
    float4x4 View;
}

EE_SAMPLERCUBE(Cubemap, 0, 1);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    const float3x3 view3x3 = (float3x3) View;
    const float4x4 view = {
        float4(view3x3[0], 0.0),
        float4(view3x3[1], 0.0),
        float4(view3x3[2], 0.0),
        float4(0.0, 0.0, 0.0, 1.0),
    };

    const float4 pos = mul(Projection, mul(view, float4(input.Position, 1.0)));
    
    output.Position = pos.xyww;
    output.TexCoord = input.Position;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = EE_TEXTURE(Cubemap, input.TexCoord);
    
    return output;
}