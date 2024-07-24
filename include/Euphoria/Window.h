#pragma once

#include <SDL.h>
#include <string>

#include "Euphoria/Math/Math.h"

namespace Euphoria {

    struct WindowInfo {
        Math::Size<int32_t> Size;
        std::string Title;
    };

    class Window {
    private:
        SDL_Window* _window;

    public:
        Window(const WindowInfo& info);
        ~Window();
    };

}
