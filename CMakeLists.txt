cmake_minimum_required(VERSION 3.28)
project(Euphoria)

set(CMAKE_CXX_STANDARD 20)

add_library(Euphoria STATIC
        include/Euphoria/Math/Size.h
        src/Window.cpp
        include/Euphoria/Window.h
        include/Euphoria/Math/Math.h
)

target_include_directories(Euphoria PRIVATE include/)

# ---------- SDL2 ----------
add_subdirectory(lib/SDL)

if (TARGET SDL2::SDL2main)
    target_link_libraries(${PROJECT_NAME} PUBLIC SDL2::SDL2main)
endif ()

target_link_libraries(${PROJECT_NAME} PUBLIC SDL2::SDL2)