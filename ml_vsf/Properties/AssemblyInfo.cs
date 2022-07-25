using System.Reflection;

[assembly: AssemblyTitle("VSFExtension")]
[assembly: AssemblyVersion("1.1.2")]
[assembly: AssemblyFileVersion("1.1.2")]

[assembly: MelonLoader.MelonInfo(typeof(ml_vsf.VsfExtension), "VSFExtension", "1.1.2", "SDraw", "https://github.com/SDraw/ml_mods")]
[assembly: MelonLoader.MelonGame("VRChat", "VRChat")]
[assembly: MelonLoader.MelonAdditionalDependencies("VRChatUtilityKit", "UIExpansionKit")]
[assembly: MelonLoader.MelonPlatform(MelonLoader.MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonLoader.MelonPlatformDomain(MelonLoader.MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]