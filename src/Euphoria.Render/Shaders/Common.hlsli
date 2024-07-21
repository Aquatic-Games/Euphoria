#pragma once

#define EE_SAMPLER2D(SamplerName, Binding, Set) \
    [[vk::combinedImageSampler]] Texture2D SamplerName : register(t##Binding, space##Set); \
    [[vk::combinedImageSampler]] SamplerState SamplerName##Sampler : register(t##Binding, space##Set);

#define EE_TEXTURE(SamplerName, TexCoord) SamplerName.Sample(SamplerName##Sampler, TexCoord)