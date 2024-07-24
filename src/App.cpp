#include "Euphoria/App.h"

namespace Euphoria {
    App::App(const LaunchInfo& info) {
        Window = std::make_unique<Euphoria::Window>(info.Window);
    }

    void App::ProcessEvents() {
        SDL_Event event;
        while (SDL_PollEvent(&event)) {
            switch (event.type) {
                case SDL_WINDOWEVENT: {
                    switch (event.window.event) {
                        case SDL_WINDOWEVENT_CLOSE:
                            IsAlive = false;
                            break;
                    }

                    break;
                }
            }
        }
    }

    void App::Run() {
        Initialize();

        IsAlive = true;

        while (IsAlive) {
            ProcessEvents();

            Update(1 / 60.0f);
            Tick(1 / 60.0f);
            Draw();
        }
    }

    void App::Initialize() {

    }

    void App::Update(float dt) {

    }

    void App::Tick(float dt) {

    }

    void App::Draw() {

    }
}