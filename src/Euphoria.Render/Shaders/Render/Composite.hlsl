#pragma pixel PSMain
#pragma debug
#include "../QuadDraw.hlsl"

struct PSOutput
{
    float4 Color: SV_Target0;
};

Texture2D Texture : register(t0, space0);
SamplerState Sampler : register(s0, space0);

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = Texture.Sample(Sampler, input.TexCoord);
    
    return output;
}
