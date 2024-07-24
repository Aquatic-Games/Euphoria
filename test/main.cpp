#include <Euphoria/App.h>

using namespace Euphoria;

int main(int argc, char* argv[]) {
    LaunchInfo info {
        .AppName = "Test",
        .Window = {
            .Size = { 1280, 720 },
            .Title = "Test"
        }
    };

    App app(info);

    app.Run();

    return 0;
}