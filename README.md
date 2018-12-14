# .NET Profiler DLL Hijack

## Background

The .NET Framework can be coerced into loading a profiling DLL into any .NET assembly when launched. This is done when a handful of environment variables and registry keys are set. For a full write-up you can view this blog here: https://offsec.provadys.com/UAC-bypass-dotnet.html

## Building and Using

The "Payload" project holds the main DLL to drop to disk. This DLL simply executes the specified command (but could be more). In the case of this project it starts mshta.exe pointing to an HTA file dropped to disk. If you want to change the command ran, you'll have to edit dllmain.cpp.

The "DNH" project is the staging executable. For this project, you should ADD two resources: the Payload.dll and the HTA file you wish to execute. The DNH project prepares the environment variables, writes the payload dll and evil hta to disk, then launches a process that uses the .NET framework. After it's been executed it cleans up the registry keys and environment variables set. You should see the HTA running with elevated rights. To change the staging process, edit Program.cs as required.

Your command doesn't have to involve any HTAs, it can be any arbitrary code. This was just the quickest way I found to weaponize it. Once DNH.exe is built, simply issue `execute-assembly` from beacon or otherwise to execute.
