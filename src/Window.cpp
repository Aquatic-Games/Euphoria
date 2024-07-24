#include "Euphoria/Window.h"

#include <stdexcept>
#include <SDL_syswm.h>

namespace Euphoria {
    Window::Window(const WindowInfo& info) {
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_EVENTS) < 0) {
            throw std::runtime_error("Window: Failed to initialize SDL: " + std::string(SDL_GetError()));
        }

        _window = SDL_CreateWindow(info.Title.c_str(), SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, info.Size.Width, info.Size.Height, SDL_WINDOW_SHOWN);

        if (!_window) {
            throw std::runtime_error("Window: Failed to create window: " + std::string(SDL_GetError()));
        }
    }

    Window::~Window() {
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }

    SDL_Window* Window::Handle() const {
        return _window;
    }

    intptr_t Window::HWND() const {
        SDL_SysWMinfo info;
        SDL_VERSION(&info.version);
        SDL_GetWindowWMInfo(_window, &info);

        return (intptr_t) info.info.win.window;
    }

    Math::Size<int32_t> Window::Size() const {
        Math::Size<int32_t> size;
        SDL_GetWindowSize(_window, &size.Width, &size.Height);

        return size;
    }

    void Window::SetSize(const Math::Size<int32_t>& size) {
        SDL_SetWindowSize(_window, size.Width, size.Height);
    }

    Math::Size<int32_t> Window::SizeInPixels() const {
        Math::Size<int32_t> size;
        SDL_GetWindowSizeInPixels(_window, &size.Width, &size.Height);

        return size;
    }
}