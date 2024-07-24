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

    App::Run(info);

    return 0;
}