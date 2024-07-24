#pragma once

#include <memory>

namespace Euphoria::Render {
    class Graphics {
    public:
        virtual ~Graphics() = default;

        virtual void Present() = 0;

        static std::unique_ptr<Graphics> CreateD3D11(intptr_t hwnd, uint32_t width, uint32_t height);
    };
}
