#include <iostream>

#include <Euphoria/App.h>

using namespace Euphoria;
using namespace Euphoria::Render;

class MyApp : public App {
public:
    explicit MyApp(const LaunchInfo& info) : App(info) {}

    void Initialize() override {
        Bitmap bitmap = Bitmap(R"(C:\Users\ollie\Pictures\awesomeface.png)");
        std::cout << bitmap.Size().ToString() << std::endl;
    }
};

int main(int argc, char* argv[]) {
    LaunchInfo info {
        .AppName = "Test",
        .Window = {
            .Size = { 1280, 720 },
            .Title = "Test"
        }
    };

    MyApp app(info);

    app.Run();

    return 0;
}