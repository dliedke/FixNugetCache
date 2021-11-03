# FixNugetCache
Analyze and repair broken NuGet packages local cache.

This is to avoid cleaning up the whole NuGet cache and downloading again all packages when there is a corrupted .nuspec file.
This project will detect and only delete the broken packages that can be restored with VS.NET

Usually when restoring NuGet packages the following issue can happen:
Error Invalid character in the given encoding. Line 1, position 1.

Related NuGet Issue:
<a href="https://github.com/NuGet/Home/issues/11037" target="_blank">https://github.com/NuGet/Home/issues/11037</a>

Related feature request:
<a href="https://github.com/NuGet/Home/issues/11045" target="_blank">https://github.com/NuGet/Home/issues/11045</a>

Download binaries already compiled to run with .NET 5.0 from:
https://github.com/dliedke/FixNugetCache/releases/tag/1.0

