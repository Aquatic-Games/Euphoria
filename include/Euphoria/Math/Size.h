#pragma once

#include <string>

namespace Euphoria::Math {

    template<typename T>
    struct SizeT {
        T Width;
        T Height;

        SizeT() {
            Width = 0;
            Height = 0;
        }

        explicit SizeT(const T wh) {
            Width = wh;
            Height = wh;
        }

        SizeT(const T width, const T height) {
            Width = width;
            Height = height;
        }

        [[nodiscard]] std::string ToString() const {
            return std::to_string(Width) + 'x' + std::to_string(Height);
        }
    };

    typedef SizeT<int32_t> Size;
    typedef SizeT<float> SizeF;
    typedef SizeT<double> SizeD;

}