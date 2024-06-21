#pragma pixel PSMain
#pragma debug
#include "../QuadDraw.hlsl"

struct PSOutput
{
    float4 Color: SV_Target0;
};

SamplerState Sampler : register(s0, space0);
Texture2D Albedo   : register(t0, space0);
Texture2D Position : register(t1, space0);

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    const float4 albedo = Albedo.Sample(Sampler, input.TexCoord);
    if (albedo.a < 0.5)
        discard;
    
    output.Color = albedo;
    
    return output;
}