#pragma once

namespace Euphoria::Render {
    class Graphics {
    public:
        virtual ~Graphics() = default;

        virtual void Present() = 0;
    };
}
