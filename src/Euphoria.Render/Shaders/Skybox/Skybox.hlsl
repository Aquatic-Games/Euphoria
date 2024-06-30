#pragma vertex VSMain
#pragma pixel PSMain
#pragma debug

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

TextureCube Cubemap  : register(t0, space1);
SamplerState Sampler : register(s0, space1);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Projection, mul(View, float4(input.Position, 1.0)));
    output.TexCoord = input.Position;
    
    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    output.Color = Cubemap.Sample(Sampler, input.TexCoord);
    
    return output;
}