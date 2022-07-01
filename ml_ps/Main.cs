using System;
using UnityEngine;

namespace ml_ps
{
    public class PanoramaScreenshot : MelonLoader.MelonMod
    {
        public override void OnApplicationStart()
        {
            MethodsResolver.ResolveMethods();
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

            Utils.PlayCameraShutterSound();
            try
            {
                PrepareCameraLayers(l_camera);
                TakeScreenshot(l_camera);
                Utils.PlayXyloSound();
            }
            catch(Exception e)
            {
                Utils.PlayBlockedSound();
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

                Utils.PlayCameraShutterSound();
                try
                {
                    l_camera.transform.parent = Utils.GetStreamCamera().transform;
                    l_camera.transform.localPosition = Vector3.zero;
                    l_camera.transform.localRotation = Quaternion.identity;

                    l_camera.fieldOfView = Utils.GetStreamCamera().GetComponent<Camera>().fieldOfView;
                    l_camera.cullingMask = Utils.GetStreamCamera().GetComponent<Camera>().cullingMask;
                    l_camera.stereoTargetEye = StereoTargetEyeMask.None;

                    PrepareCameraLayers(l_camera);

                    l_camera.transform.parent = Utils.GetUserCamera().transform;
                    l_camera.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                    Utils.GetPhotoCamera().active = false;

                    TakeScreenshot(l_camera);

                    Utils.PlayXyloSound();
                }
                catch(Exception e)
                {
                    Utils.PlayBlockedSound();
                    Logger.Warning(e.Message);
                }

                UnityEngine.Object.DestroyImmediate(l_camera.gameObject);

                RenderTexture.active = l_activeRt;
                Utils.GetPhotoCamera().active = true;
            }
            else
            {
                Utils.PlayBlockedSound();
            }
        }

        void PrepareCameraLayers(Camera p_camera)
        {
            if(Settings.IgnorePlayer)
            {
                p_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerLocal"));
                p_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("MirrorReflection")); // Player's clone
            }
            if(Settings.IgnorePlayers)
                p_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));
            if(Settings.IgnoreUI)
            {
                p_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Interactive"));
                p_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UiMenu"));
                p_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
                p_camera.cullingMask &= ~(1 << LayerMask.NameToLayer("InternalUI"));
            }
        }

        void TakeScreenshot(Camera p_camera)
        {
            RenderTexture l_rtA = new RenderTexture((int)Settings.CubemapSize, (int)Settings.CubemapSize, 24, RenderTextureFormat.ARGB32, 0);
            l_rtA.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            p_camera.RenderToCubemap(l_rtA);

            RenderTexture l_rtB = new RenderTexture((int)Settings.PanoramaWidth, (int)Settings.PanoramaHeight, 0, RenderTextureFormat.ARGB32, 0);
            l_rtB.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
            l_rtA.ConvertToEquirect(l_rtB, Camera.MonoOrStereoscopicEye.Mono);

            UnityEngine.Object.DestroyImmediate(l_rtA); // Yeet!

            RenderTexture.active = l_rtB;
            Texture2D l_resultTex = new Texture2D((int)Settings.PanoramaWidth, (int)Settings.PanoramaHeight, TextureFormat.RGB24, false);
            l_resultTex.ReadPixels(new Rect(0, 0, (int)Settings.PanoramaWidth, (int)Settings.PanoramaHeight), 0, 0);
            l_resultTex.Apply();

            UnityEngine.Object.DestroyImmediate(l_rtB); // Yeet!

            DateTime l_now = DateTime.Now;
            CheckDirectories(l_now.Year, l_now.Month);

            byte[] l_bytes = ImageConversion.EncodeToPNG(l_resultTex);
            System.IO.File.WriteAllBytes(string.Format("{0}/VRChat/{1}-{2}/{1}-{2}-{3}_{4}-{5}-{6}.png",
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                l_now.Year, l_now.Month.ToString("00"), l_now.Day.ToString("00"),
                l_now.Hour.ToString("00"), l_now.Minute.ToString("00"), l_now.Second.ToString("00")),
                l_bytes
            );

            UnityEngine.Object.DestroyImmediate(l_resultTex);
        }

        static void CheckDirectories(int p_year, int p_month)
        {
            string l_picturesDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            l_picturesDir += "/VRChat";
            if(!System.IO.Directory.Exists(l_picturesDir))
                System.IO.Directory.CreateDirectory(l_picturesDir);

            l_picturesDir += string.Format("/{0}-{1}", p_year, p_month.ToString("00"));
            if(!System.IO.Directory.Exists(l_picturesDir))
                System.IO.Directory.CreateDirectory(l_picturesDir);
        }
    }
}
