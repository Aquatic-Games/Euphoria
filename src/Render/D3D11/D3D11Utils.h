#pragma once

#include <d3d11.h>
#include <string>
#include <stdexcept>

#include "Euphoria/Render/Format.h"

namespace Euphoria::Render::D3D11::D3D11Utils {
    void CheckResult(HRESULT result, const std::string& operation);
    void CheckResult(HRESULT result);

    DXGI_FORMAT FormatToD3D(Format format);
}