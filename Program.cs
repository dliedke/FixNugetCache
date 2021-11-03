using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FixNugetCache
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Make sure to close all VS.NET instances.");
            Console.WriteLine("Analyzing and fixing Nuget packages cache issues... Please wait...");

            List<string> corruptedPackagesList = new();

            // Get the nuget package cache path for the current user
            //string nugetCachePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.nuget\packages");
            string nugetCachePath = Environment.ExpandEnvironmentVariables(@"C:\Temp\packages");

            // Search for all nuspec files
            string[] nuspecFiles = Directory.GetFiles(nugetCachePath, "*.nuspec", SearchOption.AllDirectories);

            // Go throgh all nuspec files
            foreach (string file in nuspecFiles)
            {
                // Check if it a valid XML
                if (!IsValidXml(file))
                {
                    // If not valid, then get the full package directory
                    Regex regex = new(@"(.+)packages(.)+?\\");
                    Match match = regex.Match(file);
                    if (match.Success)
                    {
                        string corruptedPackagePath = match.Value;

                        // Delete the corrupted package and add to the list
                        DirectoryInfo di = new(Path.GetDirectoryName(corruptedPackagePath));
                        try
                        {
                            di.Delete(true);
                            corruptedPackagesList.Add(corruptedPackagePath);
                        }
                        catch (Exception ex)
                        {
                            corruptedPackagesList.Add($"Error deleting folder: {corruptedPackagePath} --> {ex.GetErrorMsg()}");
                        }
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine();

            // If we had corrupted packages
            bool errors = false;
            if (corruptedPackagesList.Count > 0)
            {
                // Show all corrupted packages that were deleted to the user
                foreach (string file in corruptedPackagesList)
                {
                    if (!file.StartsWith("Error"))
                    {
                        Console.WriteLine($"Package {Path.GetDirectoryName(file)} is corrupted and it was deleted.");
                    }
                    else
                    {
                        errors = true;
                        Console.WriteLine(file);
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                // No issues found
                Console.WriteLine("No issues found Nuget packages cache.");
            }

            Console.WriteLine();
            Console.WriteLine();

            if (!errors)
            {
                // Complete fixing issues
                Console.WriteLine("COMPLETE checking/fixing Nuget packages cache issues.");
            }
            else
            {
                // Not all issues could be fixed
                Console.WriteLine("ERRORS when checking/fixing Nuget packages cache issues.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        public static bool IsValidXml(string xmlFileName)
        {
            XmlReader reader = null;

            try
            {
                bool errored = false;

                // Set the validation settings.
                XmlReaderSettings settings = new();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += ((sender, e) => { errored = e.Severity == System.Xml.Schema.XmlSeverityType.Error; });

                // Create the XmlReader object.
                reader = XmlReader.Create(xmlFileName, settings);

                // Parse the file.
                while (reader.Read());

                return !errored;
            }
            catch
            {
                return false;
            }
            finally
            {
                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch { }
            }
        }
    }
}