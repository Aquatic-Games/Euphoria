#pragma once

#include <vector>
#include <cstdint>
#include <string>

#include "Euphoria/Math/Size.h"
#include "Format.h"

namespace Euphoria::Render {

    class Bitmap {
    private:
        std::vector<uint8_t> _data;
        Math::Size _size;
        Format _format;

    public:
        Bitmap(std::vector<uint8_t> data, const Math::Size& size, Format format);
        explicit Bitmap(const std::string& path);

        [[nodiscard]] const std::vector<uint8_t>& Data() const;
        [[nodiscard]] Math::Size Size() const;
        [[nodiscard]] Format Format() const;
    };

}
