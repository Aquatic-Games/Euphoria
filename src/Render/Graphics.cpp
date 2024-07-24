#include "Euphoria/Render/Graphics.h"

#include "D3D11/D3D11Graphics.h"

namespace Euphoria::Render {
    std::unique_ptr<Graphics> Graphics::CreateD3D11(intptr_t hwnd, uint32_t width, uint32_t height) {
        return std::make_unique<D3D11::D3D11Graphics>((HWND) hwnd, width, height);
    }
}