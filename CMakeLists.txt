cmake_minimum_required(VERSION 3.28)
project(Euphoria)

set(CMAKE_CXX_STANDARD 20)

add_library(Euphoria STATIC
        include/Euphoria/Math/Size.h
)

# ---------- SDL2 ----------
find_package(SDL2 REQUIRED)

if (TARGET SDL2::SDL2main)
    target_link_libraries(${PROJECT_NAME} PUBLIC SDL2::SDL2main)
endif ()

target_link_libraries(${PROJECT_NAME} PUBLIC SDL2::SDL2)