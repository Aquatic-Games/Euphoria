#pragma once

#include <d3d11.h>

#include "Euphoria/Render/Graphics.h"

namespace Euphoria::Render::D3D11 {

    class D3D11Graphics : public Graphics {
    private:
        IDXGIFactory1* _factory;
        IDXGISwapChain* _swapChain;
        ID3D11Texture2D* _swapChainTexture;
        ID3D11RenderTargetView* _swapChainTarget;

    public:
        ID3D11Device* Device;
        ID3D11DeviceContext* Context;

        D3D11Graphics();
        ~D3D11Graphics() override;

        void Present() override;
    };

}
