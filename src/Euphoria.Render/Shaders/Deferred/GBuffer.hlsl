#pragma vertex VSMain
#pragma pixel PSMain
#pragma debug

#include "../Utils/Common.hlsli"
#include "../Utils/Math.hlsli"

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
    float4 Color:      COLOR0;
    float3x3 TBN:      NORMAL0;
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
    float4   Position;
}

cbuffer DrawInfo : register(b0, space1)
{
    float4x4 World;
    float4 AlbedoColor;
}

EE_SAMPLER2D(Albedo, 0, 2);
EE_SAMPLER2D(Normal, 1, 2);
EE_SAMPLER2D(Metallic, 2, 2);
EE_SAMPLER2D(Roughness, 3, 2);
EE_SAMPLER2D(Occlusion, 4, 2);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    const float4 worldSpace = mul(World, float4(input.Position, 1.0));

    output.Position = mul(Projection, mul(View, worldSpace));
    output.WorldSpace = worldSpace.xyz;
    output.TexCoord = input.TexCoord;
    output.Color = input.Color * AlbedoColor;

    float3 T = normalize(mul((float3x3) World, input.Tangent));
    const float3 N = normalize(mul((float3x3) World, input.Normal));
    T = normalize(T - dot(T, N) * N);
    const float3 B = normalize(cross(N, T));

    if (dot(cross(N, T), B) < 0.0)
        T *= -1.0;
    
    output.TBN = float3x3(T, B, N);

    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    const float4 albedoTex = EE_TEXTURE(Albedo, input.TexCoord) * input.Color;
    clip(albedoTex.a - GetBayerValue(input.Position.xy));
    
    float4 normalTex = EE_TEXTURE(Normal, input.TexCoord);
    // I believe SPIR-v cross makes DX shaders essentially behave like GL shaders, meaning that only the GL normal maps
    // are correct.
    // However, DX maps are more common, so this forces the use of DX maps, and makes them work correctly.
    // TODO: Is this actually the case? Or am I being stupid here
    normalTex.g = 1.0 - normalTex.g;
    const float4 metallicTex = EE_TEXTURE(Metallic, input.TexCoord);
    const float4 roughnessTex = EE_TEXTURE(Roughness, input.TexCoord);
    const float4 occlusionTex = EE_TEXTURE(Occlusion, input.TexCoord);

    float3 normal = normalize(normalTex.rgb * 2.0 - 1.0);
    normal = normalize(mul(normal, input.TBN));
    
    output.Albedo = float4(albedoTex.rgb, 1.0);
    output.Position = float4(input.WorldSpace, 1.0);
    output.Normal = float4(normal, 1.0);
    output.MetallicRoughness = float4(metallicTex.r, roughnessTex.r, occlusionTex.r, 1.0);
    
    return output;
}