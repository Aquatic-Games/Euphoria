#pragma once

#include <SDL.h>
#include <string>

#include "Euphoria/Math/Math.h"

namespace Euphoria {

    struct WindowInfo {
        Math::Size Size;
        std::string Title;
    };

    class Window {
    private:
        SDL_Window* _window;

    public:
        explicit Window(const WindowInfo& info);
        ~Window();

        [[nodiscard]] SDL_Window* Handle() const;
        [[nodiscard]] intptr_t HWND() const;

        [[nodiscard]] Math::Size Size() const;
        void SetSize(const Math::Size& size);

        [[nodiscard]] Math::Size SizeInPixels() const;
    };

}
