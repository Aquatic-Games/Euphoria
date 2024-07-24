#pragma once

#include <string>

namespace Euphoria::Math {

    template<typename T>
    struct Size {
        T Width;
        T Height;

        Size() {
            Width = 0;
            Height = 0;
        }

        explicit Size(T wh) {
            Width = wh;
            Height = wh;
        }

        Size(T width, T height) {
            Width = width;
            Height = height;
        }

        [[nodiscard]] std::string ToString() const {
            return std::to_string(Width) + 'x' + std::to_string(Height);
        }
    };

}