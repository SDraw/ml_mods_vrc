using System.Reflection;

[assembly: AssemblyTitle("HipTrackerRotator")]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]

[assembly: MelonLoader.MelonInfo(typeof(ml_htr.Main), "HipTrackerRotator", "1.0.0", "SDraw", "https://github.com/SDraw/ml_mods")]
[assembly: MelonLoader.MelonGame("VRChat", "VRChat")]
[assembly: MelonLoader.MelonAdditionalDependencies("UIExpansionKit")]
[assembly: MelonLoader.VerifyLoaderVersion(0, 4, 3, true)]
[assembly: MelonLoader.MelonPlatform(MelonLoader.MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonLoader.MelonPlatformDomain(MelonLoader.MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
