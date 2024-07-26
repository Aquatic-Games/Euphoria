#pragma once

#include "../Utils/Math.hlsli"

//float DistGGX(const float3 n, const float3 h, const float a)
float DistGGX(const float3 n, const float3 h, const float roughness)
{
    const float a = roughness * roughness;
    
    const float a2 = a * a;
    const float nDotH = max(dot(n, h), 0.0);

    const float denom = ((nDotH * nDotH) * (a2 - 1.0) + 1.0);

    return a2 / (M_PI * denom * denom);
}

//float GeomGGXSchlick(const float nDotV, const float k)
float GeomGGXSchlick(const float nDotV, const float roughness)
{
    const float r = roughness + 1.0;
    const float k = (r * r) / 8.0;

    return nDotV / (nDotV * (1.0 - k) + k);
}

//float GeomSmith(const float3 n, const float3 v, const float3 l, const float k)
float GeomSmith(const float3 n, const float3 v, const float3 l, const float roughness)
{
    const float nDotV = max(dot(n, v), 0.0);
    const float nDotL = max(dot(n, l), 0.0);
    return GeomGGXSchlick(nDotV, roughness) * GeomGGXSchlick(nDotL, roughness);
}

float3 FresSchlick(const float cosTheta, const float3 f0)
{
    // TODO: Use UE4 equation here, it's supposed to be better.
    //return f0 + (1.0 - f0) * pow(1.0 - cosTheta, 5.0);
    return f0 + (1.0 - f0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}