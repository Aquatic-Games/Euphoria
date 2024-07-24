#pragma once

#include <d3d11.h>

#include "Euphoria/Render/Texture.h"

namespace Euphoria::Render {

    class D3D11Texture : public Texture {
    private:
        Math::Size _size;

    public:
        ID3D11Texture2D* Texture;
        ID3D11ShaderResourceView* ShaderResourceView;

        D3D11Texture(ID3D11Texture2D* texture, ID3D11ShaderResourceView* srv, const Math::Size& size);
        ~D3D11Texture() override;

        [[nodiscard]] Math::Size Size() const override;
    };

} // Euphoria
