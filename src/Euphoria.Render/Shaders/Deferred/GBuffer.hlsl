#pragma vertex VSMain
#pragma pixel PSMain
#pragma debug

#include "../Utils/Common.hlsli"

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

EE_SAMPLER2D(Albedo, 0, 2);
EE_SAMPLER2D(Normal, 1, 2);
EE_SAMPLER2D(Metallic, 2, 2);
EE_SAMPLER2D(Roughness, 3, 2);
EE_SAMPLER2D(Occlusion, 4, 2);

float3 Test(float3 normal, float3 normalTexture, float3 worldPos, float2 texCoord)
{
    float3 tangentNormal = normalTexture * 2.0 - 1.0;

    float3 q1 = ddx(worldPos);
    float3 q2 = ddy(worldPos);
    float2 st1 = ddx(texCoord);
    float2 st2 = ddy(texCoord);

    float3 N = normalize(normal);
    float3 T = normalize(q1 * st2.y - q2 * st1.y);
    float3 B = -normalize(cross(N, T));
    
    float3x3 TBN = float3x3(T, B, N);

    return normalize(mul(tangentNormal, TBN));
}

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    const float4 worldSpace = mul(World, float4(input.Position, 1.0));

    output.Position = mul(Projection, mul(View, worldSpace));
    output.WorldSpace = worldSpace.xyz;
    output.TexCoord = input.TexCoord;
    //output.Normal = mul(input.Normal, World);
    output.Normal = input.Normal;
    output.Color = input.Color;
    output.Tangent = mul(input.Tangent, World);

    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    const float4 albedoTex = EE_TEXTURE(Albedo, input.TexCoord) * input.Color;
    const float4 normalTex = EE_TEXTURE(Normal, input.TexCoord);
    const float4 metallicTex = EE_TEXTURE(Metallic, input.TexCoord);
    const float4 roughnessTex = EE_TEXTURE(Roughness, input.TexCoord);
    const float4 occlusionTex = EE_TEXTURE(Occlusion, input.TexCoord);

    float3 T = input.Tangent;
    const float3 N = input.Normal;
    
    T = normalize(T - dot(N, T) * N);
    const float3 B = cross(N, T);
    const float3x3 TBN = float3x3(T, B, N);

    const float3 normal = normalize(mul(normalTex, TBN));
    
    output.Albedo = float4(albedoTex.rgb, 1.0);
    output.Position = float4(input.WorldSpace, 1.0);
    //output.Normal = float4(normal, 1.0);
    output.Normal = float4(Test(input.Normal, normalTex.rgb, input.WorldSpace, input.TexCoord), 1.0);
    output.MetallicRoughness = float4(metallicTex.r, roughnessTex.r, occlusionTex.r, 1.0);
    
    return output;
}