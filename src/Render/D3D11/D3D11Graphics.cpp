#include "D3D11Graphics.h"
#include "D3D11Utils.h"
#include "D3D11Texture.h"

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
        _swapChainTarget->Release();
        _swapChainTexture->Release();
        _swapChain->Release();

        Context->Release();
        Device->Release();

        _factory->Release();
    }

    std::unique_ptr<Texture> D3D11Graphics::CreateTexture(const Bitmap* bitmap) {
        auto size = bitmap->Size().As<uint32_t>();
        auto format = FormatToD3D(bitmap->Format());

        D3D11_TEXTURE2D_DESC desc {
            .Width = size.Width,
            .Height = size.Height,
            .MipLevels = 0,
            .ArraySize = 1,
            .Format = format,
            .SampleDesc = { .Count = 1, .Quality = 0 },
            .Usage = D3D11_USAGE_DEFAULT,
            .BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET,
            .CPUAccessFlags = 0,
            .MiscFlags = D3D11_RESOURCE_MISC_GENERATE_MIPS
        };

        ID3D11Texture2D* texture;
        CheckResult(Device->CreateTexture2D(&desc, nullptr, &texture), "Create texture");

        Context->UpdateSubresource(texture, 0, nullptr, bitmap->Data().data(), size.Width * 4, 0);

        D3D11_SHADER_RESOURCE_VIEW_DESC srvDesc {
            .Format = format,
            .ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D,
            .Texture2D {
                .MostDetailedMip = 0,
                .MipLevels = (UINT) -1,
            }
        };

        ID3D11ShaderResourceView* srv;
        CheckResult(Device->CreateShaderResourceView(texture, &srvDesc, &srv), "Create shader resource view");

        Context->GenerateMips(srv);

        return std::make_unique<D3D11Texture>(texture, srv, bitmap->Size());
    }

    void D3D11Graphics::Present() {
        Context->OMSetRenderTargets(1, &_swapChainTarget, nullptr);
        float clearColor[] = { 1.0f, 0.5f, 0.25f, 1.0f };
        Context->ClearRenderTargetView(_swapChainTarget, clearColor);

        CheckResult(_swapChain->Present(1, 0), "Present");
    }
}