#include "File.h"

#include <fstream>

namespace cge
{
    std::vector<u8> File::ReadBytes(const std::string_view& path)
    {
        // todo find a way to do this without allocating a string?
        std::string pathStr(path);
        std::ifstream stream(pathStr);
        if (!stream.is_open())
            throw std::runtime_error("Failed to open file.");

        stream.seekg(0, std::ios::end);
        size_t size = stream.tellg();
        stream.seekg(0, std::ios::beg);

        std::vector<u8> vector(size);
        stream.read(reinterpret_cast<char*>(vector.data()), size);
        return vector;
    }
}
