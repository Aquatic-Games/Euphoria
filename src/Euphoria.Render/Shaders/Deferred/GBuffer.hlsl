﻿#pragma vertex VSMain
#pragma pixel PSMain
#pragma debug

struct VSInput
{
    float3 Position: POSITION0;
    float2 TexCoord: TEXCOORD0;
    float4 Color:    COLOR0;
    float3 Normal:   NORMAL0;
};

struct VSOutput
{
    float4 Position:   SV_Position;
    float3 WorldSpace: POSITION0;
    float2 TexCoord:   TEXCOORD0;
    float3 Color:      COLOR0;
};

struct PSOutput
{
    float4 Albedo:   SV_Target0;
    float4 Position: SV_Target1;
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
Texture2D Albedo : register(t0, space2);

VSOutput VSMain(const in VSInput input)
{
    VSOutput output;

    const float4 worldSpace = mul(World, float4(input.Position, 1.0));

    output.Position = mul(Projection, mul(View, worldSpace));
    output.WorldSpace = worldSpace.xyz;
    output.TexCoord = input.TexCoord;
    output.Color = input.Color.rgb;

    return output;
}

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    float4 albedo = Albedo.Sample(Sampler, input.TexCoord);
    output.Albedo = float4(albedo.rgb * input.Color, 1.0);

    output.Position = float4(input.WorldSpace, 1.0);
    
    return output;
}