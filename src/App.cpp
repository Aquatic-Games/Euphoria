#include "Euphoria/App.h"

namespace Euphoria {
    App::App(const LaunchInfo& info) {
        Window = std::make_unique<Euphoria::Window>(info.Window);
    }

    void App::Run(const LaunchInfo& info) {
        App app(info);
    }
}