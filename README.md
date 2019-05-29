# Cake.BitDiffer

This [Cake](https://cakebuild.net/) Addin allows to use [BitDiffer](https://github.com/bitdiffer/bitdiffer) command line in your Cake build scripts.

## States

| Service | Last | Develop | Master |
| :------ | ---: | ------: | -----: |
| AppVeyor last | [![Build status last](https://ci.appveyor.com/api/projects/status/2yhsf4jsgh6u9l35?svg=true)](https://ci.appveyor.com/project/WebDucer/cake-bitdiffer) | [![Build status develop](https://ci.appveyor.com/api/projects/status/2yhsf4jsgh6u9l35/branch/develop?svg=true)](https://ci.appveyor.com/project/WebDucer/cake-bitdiffer/branch/develop) | [![Build status master](https://ci.appveyor.com/api/projects/status/2yhsf4jsgh6u9l35/branch/master?svg=true)](https://ci.appveyor.com/project/WebDucer/cake-bitdiffer/branch/master)
| SonarCube coverage | | [![SonarQube Coverage](https://sonarcloud.io/api/project_badges/measure?branch=develop&project=Cake.BitDiffer&metric=coverage)](https://sonarcloud.io/dashboard?branch=develop&id=Cake.BitDiffer) | [![SonarQube Coverage](https://sonarcloud.io/api/project_badges/measure?project=Cake.BitDiffer&metric=coverage)](https://sonarcloud.io/dashboard?id=Cake.BitDiffer) |
| SonarCube technical debt | | [![SonarQube Technical Debt](https://sonarcloud.io/api/project_badges/measure?branch=develop&project=Cake.BitDiffer&metric=sqale_index)](https://sonarcloud.io/dashboard?branch=develop&id=Cake.BitDiffer) | [![SonarQube Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=Cake.BitDiffer&metric=sqale_index)](https://sonarcloud.io/dashboard?id=Cake.BitDiffer) |
| Nuget |  [![NuGet](https://img.shields.io/nuget/dt/Cake.BitDiffer.svg)](https://www.nuget.org/packages/Cake.BitDiffer) | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Cake.BitDiffer.svg)](https://www.nuget.org/packages/Cake.BitDiffer) | [![NuGet](https://img.shields.io/nuget/v/Cake.BitDiffer.svg)](https://www.nuget.org/packages/Cake.BitDiffer) |

## Usage

To use the addin, add the following lines in your build script:

```csharp
#tool "nuget:?package=BitDiffer"

#addin "nuget:?package=Cake.BitDiffer"
```

You can set the BitDiffer settings for execution and analyse the comparison result.

```csharp
var settings = new BitDifferSettings {
    PreviousAssemblyFile = "./Version1/MyAsembly.dll",
    CurrentAssemblyFile = "./Version2/MyAssembly.dll",
    ReportOnlyChanged = true,
    CompareOnlyPublic = true,
    CompareImplementation = false,
    CompareAssemblyAttributeChanges = true,
    ResultOutputFile = "./CompareResults.xml", // Or HTML
    IsolationLevel = IsolationLevel.High,
    PreferGacVersion = true,
    ReflectionOnlyLoading = false
};
var compareResult = BitDiffer(settings);
Information(compareResult.HasChanges());
Information(compareResult.GetChangeMessage()); // Short message of detected change or error
Information(compareResult.RawResult); // Full analysis result
Information(string.Join(Environment.NewLine + ">>> ", comparisonResult.ExecutionResult)); // Content of standard output

// Or with inline configuration
var result = BitDiffer(options => {
    options.PreviousAssemblyFile = "./Version1/MyAsembly.dll";
    options.CurrentAssemblyFile = "./Version2/MyAssembly.dll"
});
```
