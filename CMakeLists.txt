cmake_minimum_required(VERSION 3.28)
project(Euphoria)

set(CMAKE_CXX_STANDARD 20)

add_library(Euphoria STATIC
        include/Euphoria/Math/Size.h
        src/Window.cpp
        include/Euphoria/Window.h
        include/Euphoria/Math/Math.h
        src/App.cpp
        include/Euphoria/App.h
        include/Euphoria/Render/Graphics.h
        src/Render/D3D11/D3D11Graphics.cpp
        src/Render/D3D11/D3D11Graphics.h
        src/Render/D3D11/D3D11Utils.cpp
        src/Render/D3D11/D3D11Utils.h
)

target_include_directories(Euphoria PRIVATE include/)

# ---------- SDL2 ----------
add_subdirectory(lib/SDL)

if (TARGET SDL2::SDL2main)
    target_link_libraries(${PROJECT_NAME} PUBLIC SDL2::SDL2main)
endif ()

target_link_libraries(${PROJECT_NAME} PUBLIC SDL2::SDL2)

# ---------- D3D11 ----------
target_link_libraries(${PROJECT_NAME} PUBLIC d3d11 dxgi d3dcompiler)

# ----- TEST -----
add_executable(EuphoriaTest test/main.cpp)
target_link_libraries(EuphoriaTest PRIVATE Euphoria)
target_include_directories(EuphoriaTest PRIVATE include/)