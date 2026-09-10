#pragma once

#include "Math/Coredefs.h"

#include <vector>
#include <string_view>

namespace cge
{
    class File final
    {
        static std::vector<u8> ReadBytes(const std::string_view& path);
    };
}
