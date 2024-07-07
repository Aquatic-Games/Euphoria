#pragma vertex VSMain
#pragma debug

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
};

[[vk::constant_id(0)]] const bool Flip = false;

VSOutput VSMain(const in uint vertex: SV_VertexID)
{
    VSOutput output;

    const float2 vertices[] = {
        float2(-1.0f, -1.0f),
        float2(+1.0f, -1.0f),
        float2(+1.0f, +1.0f),
        float2(-1.0f, +1.0f)
    };

    const float2 texCoords[] = {
        float2(0.0f, 1.0f),
        float2(1.0f, 1.0f),
        float2(1.0f, 0.0f),
        float2(0.0f, 0.0f)
    };

    const uint indices[] = {
        0, 1, 3,
        1, 2, 3
    };

    output.Position = float4(vertices[indices[vertex]], 0.0, 1.0);
    output.TexCoord = texCoords[indices[vertex]];

    if (Flip)
        output.TexCoord.y = 1.0 - output.TexCoord.y;
    
    return output;
}