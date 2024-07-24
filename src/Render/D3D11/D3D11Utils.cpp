#include "D3D11Utils.h"

#include <stdexcept>

namespace Euphoria::Render::D3D11 {
    void D3D11Utils::CheckResult(HRESULT result, const std::string& operation) {
        if (FAILED(result)) {
            throw std::runtime_error(
                    "D3D11: Operation '" + operation + "' failed with HRESULT " + std::to_string(result));
        }
    }

    void D3D11Utils::CheckResult(HRESULT result) {
        if (FAILED(result)) {
            throw std::runtime_error("D3D11: Operation failed with HRESULT " + std::to_string(result));
        }
    }
}