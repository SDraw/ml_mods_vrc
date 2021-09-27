using System.Reflection;

[assembly: AssemblyTitle("HeadTurn")]
[assembly: AssemblyVersion("1.1.2")]
[assembly: AssemblyFileVersion("1.1.2")]

[assembly: MelonLoader.MelonInfo(typeof(ml_ht.HeadTurn), "HeadTurn", "1.1.2", "SDraw", "https://github.com/SDraw/ml_ht")]
[assembly: MelonLoader.MelonGame("VRChat", "VRChat")]
[assembly: MelonLoader.MelonAdditionalDependencies("VRChatUtilityKit")]
[assembly: MelonLoader.VerifyLoaderVersion(0, 4, 3, true)]
[assembly: MelonLoader.MelonPlatform(MelonLoader.MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonLoader.MelonPlatformDomain(MelonLoader.MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
