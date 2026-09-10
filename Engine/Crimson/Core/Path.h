#pragma once

#include <string>

namespace cge
{
    class Path final
    {
    public:
#if CGE_PLATFORM_WINDOWS
        static constexpr char PathSeparator = '\\';
#else
        static constexpr char PathSeparator = '/';
#endif

        static std::string Combine(const std::string& base, const std::string& path1)
        {
            return base + PathSeparator + path1;
        }

        static std::string Combine(const std::string& base, const std::string& path1, const std::string& path2)
        {
            return base + PathSeparator + path1 + PathSeparator + path2;
        }

        static std::string Combine(const std::string& base, const std::string& path1, const std::string& path2, const std::string& path3)
        {
            return base + PathSeparator + path1 + PathSeparator + path2 + PathSeparator + path3;
        }
    };
}