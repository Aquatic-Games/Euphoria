#pragma once

#include <memory>
#include <string>

#include "Window.h"

namespace Euphoria {

    struct LaunchInfo {
        std::string AppName;
        WindowInfo Window;
    };

    class App {
    private:
        explicit App(const LaunchInfo& info);

    public:
        std::unique_ptr<Window> Window;

        static void Run(const LaunchInfo& info);
    };

}
