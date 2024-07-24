#include "D3D11Graphics.h"
#include "D3D11Utils.h"

namespace Euphoria::Render::D3D11 {
    using namespace D3D11Utils;

    D3D11Graphics::D3D11Graphics(HWND hwnd, UINT width, UINT height) {
        CheckResult(CreateDXGIFactory1(__uuidof(IDXGIFactory1), (void**) &_factory), "Create DXGI factory");

        const UINT flags = D3D11_CREATE_DEVICE_BGRA_SUPPORT | D3D11_CREATE_DEVICE_DEBUG;
        D3D_FEATURE_LEVEL level = D3D_FEATURE_LEVEL_11_0;

        CheckResult(D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, flags, &level, 1,
                                      D3D11_SDK_VERSION, &Device,nullptr, &Context), "Create device");

        DXGI_SWAP_CHAIN_DESC swapChainDesc {
            .BufferDesc = { .Width = width, .Height = height, .Format = DXGI_FORMAT_B8G8R8A8_UNORM },
            .SampleDesc = { .Count = 1, .Quality = 0 },
            .BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
            .BufferCount = 2,
            .OutputWindow = hwnd,
            .Windowed = TRUE,
            .SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
            .Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING | DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH
        };

        CheckResult(_factory->CreateSwapChain(Device, &swapChainDesc, &_swapChain), "Create swapchain");

        CheckResult(_swapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (void**) &_swapChainTexture), "Get swapchain buffer");
        CheckResult(Device->CreateRenderTargetView(_swapChainTexture, nullptr, &_swapChainTarget), "Create swapchain target");
    }

    D3D11Graphics::~D3D11Graphics() {
        Context->Release();
        Device->Release();

        _factory->Release();
    }

    void D3D11Graphics::Present() {
        Context->OMSetRenderTargets(1, &_swapChainTarget, nullptr);
        float clearColor[] = { 1.0f, 0.5f, 0.25f, 1.0f };
        Context->ClearRenderTargetView(_swapChainTarget, clearColor);

        CheckResult(_swapChain->Present(1, 0), "Present");
    }
}