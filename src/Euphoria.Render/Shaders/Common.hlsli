#pragma once

#define EE_SAMPLER(TextureType, SamplerName, Binding, Set) \
    [[vk::combinedImageSampler]] TextureType SamplerName : register(t##Binding, space##Set); \
    [[vk::combinedImageSampler]] SamplerState SamplerName##Sampler : register(s##Binding, space##Set);

#define EE_SAMPLER2D(SamplerName, Binding, Set) EE_SAMPLER(Texture2D, SamplerName, Binding, Set)
#define EE_SAMPLERCUBE(SamplerName, Binding, Set) EE_SAMPLER(TextureCube, SamplerName, Binding, Set)

#define EE_TEXTURE(SamplerName, TexCoord) SamplerName.Sample(SamplerName##Sampler, TexCoord)