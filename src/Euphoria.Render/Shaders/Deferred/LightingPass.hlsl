#pragma pixel PSMain
#pragma debug

#include "../QuadDraw.hlsl"
#include "../Lighting/PBRbase.hlsli"

struct PSOutput
{
    float4 Color: SV_Target0;
};

SamplerState Sampler        : register(s0, space0);
Texture2D Albedo            : register(t1, space0);
Texture2D Position          : register(t2, space0);
Texture2D Normal            : register(t3, space0);
Texture2D MetallicRoughness : register(t4, space0);

PSOutput PSMain(const in VSOutput input)
{
    PSOutput output;

    float4 albedo = Albedo.Sample(Sampler, input.TexCoord);
    if (albedo.a < 0.5)
        discard;

    albedo = pow(albedo, 2.2);

    const float4 position = Position.Sample(Sampler, input.TexCoord);
    const float4 normal = Normal.Sample(Sampler, input.TexCoord);
    const float4 metallicRoughness = MetallicRoughness.Sample(Sampler, input.TexCoord);

    const float3 metallic = metallicRoughness.r;
    const float3 roughness = metallicRoughness.g;
    const float3 occlusion = metallicRoughness.b;

    // TODO: I don't think the normalize is necessary. If it is, it probably shouldn't be.
    const float3 n = normalize(normal.xyz);
    // TODO: camPos - WorldPos: Can gbuffer be simplified to contain this directly?
    const float3 TEMP_camPos = float3(0, 1.5, 2.0);
    const float3 v = normalize(TEMP_camPos - position.xyz);

    const float3 TEMP_lightDir = normalize(-float3(0.0, -1.0, 0.0));

    //const float3 l = normalize(TEMP_lightDir - position.xyz);
    const float3 l = TEMP_lightDir;
    const float3 h = normalize(v + l);

    const float3 f0 = lerp((float3) 0.04, albedo.rgb, metallic);
    const float3 f = FresSchlick(max(dot(h, v), 0.0), f0);

    const float3 kd = (1.0 - f) * 1.0 - metallic;

    const float ndf = DistGGX(n, h, roughness);
    const float g = GeomSmith(n, v, l, roughness);

    const float3 num = ndf * g * f;
    const float denom = 4.0 * max(dot(n, v), 0.0) * max(dot(n, l), 0.0) + 0.0001;
    
    const float3 specular = num / denom;

    float nDotL = max(dot(n, l), 0.0);

    float3 color = (kd * albedo.rgb / M_PI + specular) * 1.0 * nDotL;

    const float3 ambient = 0.1 * albedo.rgb * occlusion;
    color += ambient;

    color /= color + 1.0;
    color = pow(color, 1.0 / 2.2);
    
    output.Color = float4(color, 1.0);
    
    return output;
}