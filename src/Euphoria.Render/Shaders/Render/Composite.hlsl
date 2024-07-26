#pragma pixel PSMain
#pragma debug

#include "../QuadDraw.hlsl"
#include "../Utils/Common.hlsli"

struct PSOutput
{
    float4 Color: SV_Target0;
};

EE_SAMPLER2D(Texture, 0, 0);

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = EE_TEXTURE(Texture, input.TexCoord);
    
    return output;
}
