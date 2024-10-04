using System.IO;
using System;
using System.Runtime.InteropServices;


namespace Xbim.GLTF.IO
{
    internal static class XbimConfiguration
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddDllDirectory(string lpPathName);


        static XbimConfiguration()
        {
            string executingDirectory = null, nativePath = null;

            try
            {
                if (!IsWindows)
                {
                    const string notSet = "_Not_set_";
                    return;
                }

                executingDirectory = AppContext.BaseDirectory;

                if (string.IsNullOrEmpty(executingDirectory))
                    throw new InvalidOperationException("cannot get executing directory");


                // modify search place and order
                //SetDefaultDllDirectories(DllSearchFlags);

                var dllBasePath = Path.Combine(executingDirectory, "runtimes");
                nativePath = Path.Combine(dllBasePath, $"win-{GetPlatform()}", "native");
                if (!Directory.Exists(nativePath))
                    throw new DirectoryNotFoundException($"native directory not found at '{nativePath}'");
                if (!File.Exists(Path.Combine(nativePath, "gdal_wrap.dll")))
                    throw new FileNotFoundException(
                        $"GDAL native wrapper file not found at '{Path.Combine(nativePath, "gdal_wrap.dll")}'");

                // Add directories
                AddDllDirectory(nativePath);
            }
            catch
            {

            }
        }


        /// <summary>
        /// Function to determine which platform we're on
        /// </summary>
        private static string GetPlatform()
        {
            return Environment.Is64BitProcess ? "x64" : "x86";
        }

        /// <summary>
        /// Gets a value indicating if we are on a windows platform
        /// </summary>
        private static bool IsWindows
        {
            get
            {
                var res = !(Environment.OSVersion.Platform == PlatformID.Unix ||
                            Environment.OSVersion.Platform == PlatformID.MacOSX);

                return res;
            }
        }
    }
}
