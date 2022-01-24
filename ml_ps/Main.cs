using System;
using UnityEngine;

namespace ml_ps
{
    public class PanoramaScreenshot : MelonLoader.MelonMod
    {
        public override void OnApplicationStart()
        {
            Settings.Load();

            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
        }

        public override void OnPreferencesSaved()
        {
            Settings.Reload();
        }

        void OnUiManagerInit()
        {
            GameObject l_cameraTabGroup = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Camera/Scrollrect/Viewport/VerticalLayoutGroup/Buttons").gameObject;
            VRChatUtilityKit.Ui.UiManager.AddButtonToExistingGroup(l_cameraTabGroup, new VRChatUtilityKit.Ui.SingleButton(this.OnPanoramaMain, IconsHandler.GetIcon("vrpano"), "Panorama (main)", "Button_PanoramaMain", "Take panorama screenshot from main camera"));
            VRChatUtilityKit.Ui.UiManager.AddButtonToExistingGroup(l_cameraTabGroup, new VRChatUtilityKit.Ui.SingleButton(this.OnPanoramaStream, IconsHandler.GetIcon("vrpano"), "Panorama (stream)", "Button_PanoramaStream", "Take panorama screenshot from stream camera"));
        }

        void OnPanoramaMain()
        {
            RenderTexture l_activeRt = RenderTexture.active;
            Camera l_camera = Utils.GetMainCamera();
            int l_oldCullMask = l_camera.cullingMask;

            try
            {
                if(Settings.IgnorePlayer)
                    l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerLocal"));
                if(Settings.IgnorePlayers)
                    l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
                if(Settings.IgnoreUI)
                {
                    l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Interactive"));
                    l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UiMenu"));
                    l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
                    l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("InternalUI"));
                }

                RenderTexture l_rtA = new RenderTexture(Settings.CubemapSize, Settings.CubemapSize, 24, RenderTextureFormat.ARGB32);
                l_rtA.dimension = UnityEngine.Rendering.TextureDimension.Cube;
                l_camera.RenderToCubemap(l_rtA);

                RenderTexture l_rtB = new RenderTexture(Settings.PanoramaWidth, Settings.PanoramaHeight, 0, RenderTextureFormat.ARGB32);
                l_rtA.ConvertToEquirect(l_rtB, Camera.MonoOrStereoscopicEye.Mono);

                RenderTexture.active = l_rtB;
                Texture2D l_resultTex = new Texture2D(Settings.PanoramaWidth, Settings.PanoramaHeight, TextureFormat.RGB24, false);
                l_resultTex.ReadPixels(new Rect(0, 0, Settings.PanoramaWidth, Settings.PanoramaHeight), 0, 0);
                l_resultTex.Apply();

                CheckDirectories();

                byte[] l_bytes = ImageConversion.EncodeToPNG(l_resultTex);
                System.IO.File.WriteAllBytes(string.Format("{0}/VRChat/Panorama/{1}-{2}/{1}-{2}-{3} {4}-{5}-{6} ({7}).png",
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,
                    Utils.CleanupAsFilename(Utils.GetCurrentWorldName())), l_bytes
                );

                UnityEngine.Object.DestroyImmediate(l_rtA);
                UnityEngine.Object.DestroyImmediate(l_rtB);
                UnityEngine.Object.DestroyImmediate(l_resultTex);
            }
            catch(Exception e)
            {
                Logger.Warning(e.Message);
            }

            RenderTexture.active = l_activeRt;
            l_camera.cullingMask = l_oldCullMask;
        }

        void OnPanoramaStream()
        {
            if(Utils.GetStreamCamera().activeInHierarchy)
            {
                RenderTexture l_activeRt = RenderTexture.active;
                Camera l_camera = new GameObject("PanoramaCamera").AddComponent<Camera>();

                try
                {
                    l_camera.transform.parent = VRC.UserCamera.UserCameraController.field_Internal_Static_UserCameraController_0.field_Private_Camera_0.transform;
                    l_camera.transform.localPosition = Vector3.zero;
                    l_camera.transform.localRotation = Quaternion.identity;

                    l_camera.fieldOfView = Utils.GetStreamCamera().GetComponent<Camera>().fieldOfView;
                    l_camera.stereoTargetEye = StereoTargetEyeMask.None;

                    if(Settings.IgnorePlayer)
                        l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerLocal"));
                    if(Settings.IgnorePlayers)
                        l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
                    if(Settings.IgnoreUI)
                    {
                        l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Interactive"));
                        l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UiMenu"));
                        l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
                        l_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("InternalUI"));
                    }

                    l_camera.transform.parent = Utils.GetUserCamera().transform;
                    l_camera.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                    Utils.GetPhotoCamera().active = false;

                    RenderTexture l_rtA = new RenderTexture(Settings.CubemapSize, Settings.CubemapSize, 24, RenderTextureFormat.ARGB32);
                    l_rtA.dimension = UnityEngine.Rendering.TextureDimension.Cube;
                    l_camera.RenderToCubemap(l_rtA);

                    RenderTexture l_rtB = new RenderTexture(Settings.PanoramaWidth, Settings.PanoramaHeight, 0, RenderTextureFormat.ARGB32);
                    l_rtA.ConvertToEquirect(l_rtB, Camera.MonoOrStereoscopicEye.Mono);

                    RenderTexture.active = l_rtB;
                    Texture2D l_resultTex = new Texture2D(Settings.PanoramaWidth, Settings.PanoramaHeight, TextureFormat.RGB24, false);
                    l_resultTex.ReadPixels(new Rect(0, 0, Settings.PanoramaWidth, Settings.PanoramaHeight), 0, 0);
                    l_resultTex.Apply();

                    CheckDirectories();

                    byte[] l_bytes = ImageConversion.EncodeToPNG(l_resultTex);
                    System.IO.File.WriteAllBytes(string.Format("{0}/VRChat/Panorama/{1}-{2}/{1}-{2}-{3} {4}-{5}-{6} ({7}).png",
                        Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                        DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                        DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,
                        Utils.CleanupAsFilename(Utils.GetCurrentWorldName())), l_bytes
                    );

                    UnityEngine.Object.DestroyImmediate(l_rtA);
                    UnityEngine.Object.DestroyImmediate(l_rtB);
                    UnityEngine.Object.DestroyImmediate(l_resultTex);
                }
                catch(Exception e)
                {
                    Logger.Warning(e.Message);
                }

                UnityEngine.Object.DestroyImmediate(l_camera.gameObject);

                RenderTexture.active = l_activeRt;
                Utils.GetPhotoCamera().active = true;
            }
        }

        static void CheckDirectories()
        {
            string l_picturesDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            l_picturesDir += "/VRChat";
            if(!System.IO.Directory.Exists(l_picturesDir))
                System.IO.Directory.CreateDirectory(l_picturesDir);

            l_picturesDir += "/Panorama";
            if(!System.IO.Directory.Exists(l_picturesDir))
                System.IO.Directory.CreateDirectory(l_picturesDir);

            l_picturesDir += string.Format("/{0}-{1}", DateTime.Now.Year, DateTime.Now.Month);
            if(!System.IO.Directory.Exists(l_picturesDir))
                System.IO.Directory.CreateDirectory(l_picturesDir);
        }
    }
}
