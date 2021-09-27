using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ml_lme
{
    static class DependenciesHandler
    {
        static readonly List<string> ms_libraries = new List<string>()
        {
            "LeapC.dll"
        };

        public static void ExtractDependencies()
        {
            var l_assembly = Assembly.GetExecutingAssembly();
            var l_assemblyName = l_assembly.GetName().Name;

            foreach(var l_library in ms_libraries)
            {
                if(!File.Exists(l_library))
                {
                    try
                    {
                        Stream l_readStream = l_assembly.GetManifestResourceStream(l_assemblyName + "." + l_library);
                        Stream l_writeStream = File.Create(l_library);
                        l_readStream.CopyTo(l_writeStream);
                        l_writeStream.Flush();
                        l_writeStream.Close();
                        l_readStream.Close();
                    }
                    catch(Exception)
                    {
                        MelonLoader.MelonLogger.Error("Unable to extract embedded " + l_library + " library");
                    }
                }
            }
        }
    }
}
