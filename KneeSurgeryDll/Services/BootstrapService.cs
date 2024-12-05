using System.IO.Compression;
using System.Reflection;

namespace KneeSurgeryDll.Services
{
    public static class BootstrapService
    {
        public static int BootstrapExtraction()
        {
            try
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();

                foreach (string manifestResourceName in executingAssembly.GetManifestResourceNames())
                {
                    if (manifestResourceName.EndsWith("bootstrapper.zip"))
                    {
                        using (Stream stream = executingAssembly.GetManifestResourceStream(manifestResourceName))
                        {
                            using (ZipArchive zipArchive = new ZipArchive(stream))
                            {
                                zipArchive.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory, overwriteFiles: true);
                            }
                        }
                    }
                }

                return 1;
            }
            catch
            {
                return -1;
            }
        }
    }
}