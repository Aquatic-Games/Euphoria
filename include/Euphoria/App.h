#pragma once

#include <memory>
#include <string>

#include "Window.h"
#include "Render/Graphics.h"

namespace Euphoria {

    struct LaunchInfo {
        std::string AppName;
        WindowInfo Window;
    };

    class App {
    protected:
        virtual void Initialize();
        virtual void Update(float dt);
        virtual void Tick(float dt);
        virtual void Draw();

        void ProcessEvents();

    public:
        bool IsAlive;

        std::unique_ptr<Window> Window;
        std::unique_ptr<Render::Graphics> Graphics;

        explicit App(const LaunchInfo& info);
        void Run();
    };

}
