#include <utility>
#include <stdexcept>
#include <format>

#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>

#include "Euphoria/Render/Bitmap.h"

namespace Euphoria::Render {
    Bitmap::Bitmap(std::vector<uint8_t> data, const Math::Size& size, Render::Format format) {
        _data = std::move(data);
        _size = size;
        _format = format;
    }

    Bitmap::Bitmap(const std::string& path) {
        Math::Size size;
        int comp;
        void* data = stbi_load(path.c_str(), &size.Width, &size.Height, &comp, 4);

        if (!data) {
            throw std::runtime_error(std::format("Bitmap: Failed to load image: {}", stbi_failure_reason()));
        }

        _data = std::vector<uint8_t>((uint8_t*) data, (uint8_t*) data + (size.Width * size.Height * comp));
        _size = size;
        _format = Format::R8G8B8A8_UNorm;
    }

    const std::vector<uint8_t>& Bitmap::Data() const {
        return _data;
    }

    Math::Size Bitmap::Size() const {
        return _size;
    }

    Format Bitmap::Format() const {
        return _format;
    }
}