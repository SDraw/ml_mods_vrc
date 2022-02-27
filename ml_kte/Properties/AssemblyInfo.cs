using System.Reflection;

[assembly: AssemblyTitle("KinectTrackingExtension")]
[assembly: AssemblyVersion("1.2.1")]
[assembly: AssemblyFileVersion("1.2.1")]

[assembly: MelonLoader.MelonInfo(typeof(ml_kte.KinectTrackingExtension), "KinectTrackingExtension", "1.2.1", "SDraw", "https://github.com/SDraw/ml_mods")]
[assembly: MelonLoader.MelonGame("VRChat", "VRChat")]
[assembly: MelonLoader.MelonAdditionalDependencies("VRChatUtilityKit")]
[assembly: MelonLoader.MelonPlatform(MelonLoader.MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonLoader.MelonPlatformDomain(MelonLoader.MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]