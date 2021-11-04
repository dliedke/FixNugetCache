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
            try
            {
                Console.WriteLine("Make sure to close all VS.NET instances.");
                Console.WriteLine("Analyzing and fixing Nuget packages cache issues... Please wait...");

                List<string> corruptedPackagesList = new();

                // Get the nuget package cache path for the current user
                string nugetCachePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.nuget\packages");

                // Search for all nuspec files
                string[] nuspecFiles = Directory.GetFiles(nugetCachePath, "*.nuspec", SearchOption.AllDirectories);

                // Go throgh all nuspec files found
                foreach (string file in nuspecFiles)
                {
                    // Check if the .nuspec is a valid XML
                    if (!IsValidXml(file))
                    {
                        // If it not valid, then get the full package directory
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

                // Extra lines
                Console.WriteLine();
                Console.WriteLine();

                // If we found corrupted packages and they were deleted
                bool errors = false;
                if (corruptedPackagesList.Count > 0)
                {
                    // Show all corrupted packages that were deleted to the user
                    foreach (string file in corruptedPackagesList)
                    {
                        // Check if there was not an error
                        if (!file.StartsWith("Error"))
                        {
                            Console.WriteLine($"Package {Path.GetDirectoryName(file)} is corrupted and it was deleted.");
                        }
                        else
                        {
                            // There was an error deleting the folder, so show the error to the user
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
                    Console.WriteLine("Some ERRORS when checking/fixing Nuget packages cache issues.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running the application: {ex.GetErrorMsg()}");
            }

            Console.WriteLine("\r\nPress any key to exit...");
            Console.ReadLine();
        }

        public static bool IsValidXml(string xmlFileName)
        {
            XmlReader reader = null;

            try
            {
                bool errored = false;

                // Set the validation settings
                XmlReaderSettings settings = new();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += ((sender, e) => { errored = e.Severity == System.Xml.Schema.XmlSeverityType.Error; });

                // Create the XmlReader object
                reader = XmlReader.Create(xmlFileName, settings);

                // Parse the file
                while (reader.Read());

                return !errored;
            }
            catch
            {
                return false;
            }
            finally
            {
                // Close the file and dispose the reader everytime
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