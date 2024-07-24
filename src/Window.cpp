#include "Euphoria/Window.h"

#include <stdexcept>

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
}