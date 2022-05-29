using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace ml_lme
{
    static class AssetsHandler
    {
        static readonly List<string> ms_assets = new List<string>()
        {
            "leapmotion_controller.asset"
        };

        static Dictionary<string, AssetBundle> ms_loadedAssets = new Dictionary<string, AssetBundle>();
        static Dictionary<string, GameObject> ms_loadedObjects = new Dictionary<string, GameObject>();

        public static void Load()
        {
            Assembly l_assembly = Assembly.GetExecutingAssembly();
            string l_assemblyName = l_assembly.GetName().Name;

            foreach(string l_assetName in ms_assets)
            {
                try
                {
                    Stream l_assetStream = l_assembly.GetManifestResourceStream(l_assemblyName + ".resources." + l_assetName);
                    if(l_assetStream != null)
                    {
                        MemoryStream l_memorySteam = new MemoryStream((int)l_assetStream.Length);
                        l_assetStream.CopyTo(l_memorySteam);
                        AssetBundle l_assetBundle = AssetBundle.LoadFromMemory_Internal(l_memorySteam.ToArray(), 0);
                        if(l_assetBundle != null)
                        {
                            l_assetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                            ms_loadedAssets.Add(l_assetName, l_assetBundle);
                        }
                        else
                            Logger.Warning("Unable to load bundled '" + l_assetName + "' asset");
                    }
                    else
                        Logger.Warning("Unable to get bundled '" + l_assetName + "' asset stream");
                }
                catch(System.Exception e)
                {
                    Logger.Warning("Unable to load bundled '" + l_assetName + "' asset, reason: " + e.Message);
                }
            }
        }

        public static GameObject GetAsset(string p_name)
        {
            GameObject l_result = null;
            if(ms_loadedObjects.ContainsKey(p_name))
            {
                l_result = Object.Instantiate(ms_loadedObjects[p_name]);
                l_result.SetActive(true);
                l_result.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            else
            {
                foreach(var l_pair in ms_loadedAssets)
                {
                    if(l_pair.Value.Contains(p_name))
                    {
                        GameObject l_bundledObject = l_pair.Value.LoadAsset_Internal(p_name, Il2CppType.Of<GameObject>()).TryCast<GameObject>();
                        if(l_bundledObject != null)
                        {
                            ms_loadedObjects.Add(p_name, l_bundledObject);
                            l_result = Object.Instantiate(l_bundledObject);
                            l_result.SetActive(true);
                            l_result.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                        }
                        break;
                    }
                }
            }
            return l_result;
        }

        public static void Unload()
        {
            foreach(var l_pair in ms_loadedAssets)
                Object.Destroy(l_pair.Value);
            ms_loadedAssets.Clear();
        }
    }
}
