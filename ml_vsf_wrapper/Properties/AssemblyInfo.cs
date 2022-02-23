using System.Reflection;

[assembly: AssemblyTitle("VSFExtensionWrapper")]
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]

[assembly: MelonLoader.MelonInfo(typeof(ml_vsf_wrapper.VsfWrapper), "VSFExtensionWrapper", "1.0.0", "SDraw", "https://github.com/SDraw/ml_mods")]
[assembly: MelonLoader.MelonGame(null, "VSeeFace")]
[assembly: MelonLoader.MelonPlatform(MelonLoader.MelonPlatformAttribute.CompatiblePlatforms.WINDOWS_X64)]
[assembly: MelonLoader.MelonPlatformDomain(MelonLoader.MelonPlatformDomainAttribute.CompatibleDomains.MONO)]