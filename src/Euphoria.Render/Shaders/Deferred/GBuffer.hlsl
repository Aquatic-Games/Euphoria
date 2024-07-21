#pragma vertex VSMain
#pragma pixel PSMain
#pragma debug

struct VSInput
{
    float3 Position:  POSITION0;
    float2 TexCoord:  TEXCOORD0;
    float4 Color:     COLOR0;
    float3 Normal:    NORMAL0;
    float3 Tangent:   TANGENT0;
};

struct VSOutput
{
    float4 Position:   SV_Position;
    float3 WorldSpace: POSITION0;
    float2 TexCoord:   TEXCOORD0;
    float3 Normal:     NORMAL0;
    float4 Color:      COLOR0;
    float3 Tangent:    TANGENT0;
};

struct PSOutput
{
    float4 Albedo:            SV_Target0;
    float4 Position:          SV_Target1;
    float4 Normal:            SV_Target2;
    float4 MetallicRoughness: SV_Target3;
};

cbuffer CameraInfo : register(b0, space0)
{
    float4x4 Projection;
    float4x4 View;
}

cbuffer DrawInfo : register(b0, space1)
{
    float4x4 World;
}

SamplerState Sampler : register(s0, space2);
Texture2D Albedo     : register(t1, space2);
Texture2D Normal     : register(t2, space2);
Texture2D Metallic   : register(t3, space2);
Texture2D Roughness  : register(t4, space2);
Texture2D Occlusion  : register(t5, space2);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    const float4 worldSpace = mul(World, float4(input.Position, 1.0));

    output.Position = mul(Projection, mul(View, worldSpace));
    output.WorldSpace = worldSpace.xyz;
    output.TexCoord = input.TexCoord;
    output.Normal = normalize(mul(World, input.Normal));
    output.Color = input.Color;
    output.Tangent = input.Tangent;

    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    const float4 albedoTex = Albedo.Sample(Sampler, input.TexCoord) * input.Color;
    const float4 normalTex = Normal.Sample(Sampler, input.TexCoord);
    const float4 metallicTex = Metallic.Sample(Sampler, input.TexCoord);
    const float4 roughnessTex = Roughness.Sample(Sampler, input.TexCoord);
    const float4 occlusionTex = Occlusion.Sample(Sampler, input.TexCoord);

    const float3 tangent = input.Tangent;
    float3 normal = input.Normal;
    
    const float3 T = normalize(tangent - normal * dot(normal, tangent));
    const float3 B = cross(input.Normal, T);
    const float3x3 TBN = float3x3(T, B, normal);

    normal = normalize(mul(normalTex, TBN));
    
    output.Albedo = float4(albedoTex.rgb, 1.0);
    output.Position = float4(input.WorldSpace, 1.0);
    output.Normal = float4(normal, 1.0);
    output.MetallicRoughness = float4(metallicTex.r, roughnessTex.r, occlusionTex.r, 1.0);
    
    return output;
}