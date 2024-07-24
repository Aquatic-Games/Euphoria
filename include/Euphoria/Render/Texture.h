#pragma once

#include "Euphoria/Math/Math.h"

namespace Euphoria::Render {

    class Texture {
    public:
        virtual ~Texture() = 0;

        virtual Math::Size Size() const = 0;
    };

}
