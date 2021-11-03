# FixNugetCache
Analyze and repair broken NuGet packages local cache.

This is to avoid cleaning up the whole NuGet cache and downloading again all packages when there is a corrupted .nuspec file.
This project will detect and only delete the broken packages that can be restored with VS.NET

Usually when restoring NuGet packages the following issue can happen:
Error Invalid character in the given encoding. Line 1, position 1.

Related NuGet Issue:
https://github.com/NuGet/Home/issues/11037

Related feature request:
https://github.com/NuGet/Home/issues/11045
