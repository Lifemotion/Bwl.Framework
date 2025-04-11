//   Bwl.Framework.SharedMemory

//   Copyright 2025 Artem Drobanov (artem.drobanov@gmail.com), Ilya Kuryshev(sijeix2 @gmail.com)

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may Not use this file except In compliance With the License.
//   You may obtain a copy Of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law Or agreed To In writing, software
//   distributed under the License Is distributed On an "AS IS" BASIS,
//   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
//   See the License For the specific language governing permissions And
//   limitations under the License.

using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Bwl.Framework.SharedMemory;

internal static class MemoryMappedFileUniversal
{

    /// <summary>
    /// Creates or opens a memory-mapped file with the specified name and size (Windows or Linux).
    /// </summary>
    /// <param name="name">Name of memory-mapped file</param>
    /// <param name="size">Size of memory-mapped file, bytes</param>
    /// <param name="memoryMappedFileAccess">Access mode for memory-mapped file.</param>
    /// <returns>Memory mapped file supported in current OS</returns>
    /// <exception cref="NotSupportedException">Thrown if attempted to use on OS different from Windows or Linux</exception>
    internal static MemoryMappedFile CreateOrOpen(string name,
        long size,
        MemoryMappedFileAccess memoryMappedFileAccess = MemoryMappedFileAccess.ReadWrite)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return MemoryMappedFile.CreateFromFile($"/dev/shm/{name}", FileMode.OpenOrCreate, null, size, memoryMappedFileAccess);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return MemoryMappedFile.CreateOrOpen(name, size, memoryMappedFileAccess);
        else
            throw new NotSupportedException("Unsupported OS platform");
    }
}
