#include "D3D11Graphics.h"
#include "D3D11Utils.h"

namespace Euphoria::Render::D3D11 {
    using namespace D3D11Utils;

    D3D11Graphics::D3D11Graphics() {
        CheckResult(CreateDXGIFactory(__uuidof(IDXGIFactory1), (void**) &_factory), "Create DXGI factory");
    }

    D3D11Graphics::~D3D11Graphics() {
        _factory->Release();
    }

    void D3D11Graphics::Present() {

    }
}