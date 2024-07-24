#pragma once

#include <memory>

#include "Bitmap.h"
#include "Texture.h"

namespace Euphoria::Render {
    class Graphics {
    public:
        virtual ~Graphics() = default;

        virtual std::unique_ptr<Texture> CreateTexture(const Bitmap& bitmap) = 0;

        virtual void Present() = 0;

        static std::unique_ptr<Graphics> CreateD3D11(intptr_t hwnd, uint32_t width, uint32_t height);
    };
}
