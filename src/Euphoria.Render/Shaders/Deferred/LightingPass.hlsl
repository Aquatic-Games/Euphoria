#pragma vertex Vertex
#pragma pixel Pixel

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

SamplerState Sampler : register(s0, space0);
Texture2D Albedo   : register(t0, space0);
Texture2D Position : register(t1, space0);

// TODO: Split this vertex shader into a separate file, it can be reused.
VSOutput Vertex(const in uint vertex: SV_VertexID)
{
    VSOutput output;

    const float2 vertices[] = {
        float2(-1.0f, -1.0f),
        float2(+1.0f, -1.0f),
        float2(+1.0f, +1.0f),
        float2(-1.0f, +1.0f)
    };

    const float2 texCoords[] = {
        float2(0.0f, 0.0f),
        float2(1.0f, 0.0f),
        float2(1.0f, 1.0f),
        float2(0.0f, 1.0f)
    };

    const uint indices[] = {
        0, 1, 3,
        1, 2, 3
    };

    output.Position = float4(vertices[indices[vertex]], 0.0, 1.0);
    output.TexCoord = texCoords[indices[vertex]];
    
    return output;
}

PSOutput Pixel(const in VSOutput input)
{
    PSOutput output;

    const float4 albedo = Albedo.Sample(Sampler, input.TexCoord);
    if (albedo.a < 0.5)
        discard;
    
    output.Color = albedo;
    
    return output;
}