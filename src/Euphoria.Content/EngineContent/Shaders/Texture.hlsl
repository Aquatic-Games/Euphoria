struct VSInput
{
    float2 Position: POSITION0;
    float2 TexCoord: TEXCOORD0;
    float4 Tint:     COLOR0;
};

struct VSOutput
{
    float4 Position: SV_Position;
    float2 TexCoord: TEXCOORD0;
    float4 Tint:     COLOR0;
};

struct PSOutput
{
    float4 Color: SV_Target0;
};

cbuffer CameraMatrices : register(b0)
{
    float4x4 Projection;
}

VSOutput Vertex(const in VSInput input)
{
    VSOutput output;

    output.Position = mul(Projection, float4(input.Position, 0.0, 1.0));
    output.TexCoord = input.TexCoord;
    output.Tint = input.Tint;
    
    return output;
}

PSOutput Pixel(const in VSOutput input)
{
    PSOutput output;

    output.Color = input.Tint;
    
    return output;
}