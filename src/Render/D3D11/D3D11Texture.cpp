#include "D3D11Texture.h"

namespace Euphoria::Render {
    D3D11Texture::D3D11Texture(ID3D11Texture2D* texture, ID3D11ShaderResourceView* srv, const Math::Size& size) {
        Texture = texture;
        ShaderResourceView = srv;
        _size = size;
    }

    D3D11Texture::~D3D11Texture() {
        ShaderResourceView->Release();
        Texture->Release();
    }

    Math::Size D3D11Texture::Size() const {
        return _size;
    }
}