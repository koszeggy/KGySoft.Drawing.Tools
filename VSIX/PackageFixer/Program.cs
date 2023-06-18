using System.IO.Compression;

using KGySoft.CoreLibraries;
using KGySoft.Json;

namespace PackageFixer
{
    internal static class Program
    {
        private const string contentTypesXml = "[Content_Types].xml";
        private const string catalogJson = "catalog.json";
        private const string manifestJson = "manifest.json";
        private static readonly string[] specialEntries = { contentTypesXml, catalogJson, manifestJson };

        static void Main(string[] args)
        {
            Console.WriteLine($".VSIX package fixer v{typeof(Program).Assembly.GetName().Version}");
            Console.WriteLine();
            if (args.Length != 2)
            {
                Console.WriteLine("Use this tool to clean-up .vsix file if VisualStudio build works incorrectly.");
                Console.WriteLine("See https://developercommunity.visualstudio.com/t/ExtensionA-lot-of-unnecessary-files-ap/10390551");
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine("  PackageFixer <wrong .vsix> <reference .vsix>");
                return;
            }

            try
            {
                DoFix(args[0], args[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: failed to perform the fix: {e}");
            }
        }

        private static void DoFix(string dstFile, string refFile)
        {
            if (!File.Exists(dstFile))
                throw new FileNotFoundException($"Target package does not exist: {dstFile}", dstFile);
            if (!File.Exists(refFile))
                throw new FileNotFoundException($"Reference package does not exist: {refFile}", refFile);

            // analyzing the reference file
            Console.WriteLine($"Analyzing reference {Path.GetFileName(refFile)}...");
            var refEntries = new HashSet<string>();
            byte[]? contentTypes;
            using (ZipArchive refVsix = ZipFile.OpenRead(refFile))
            {
                refEntries.AddRange(refVsix.Entries.Select(e => e.FullName));
                contentTypes = refVsix.GetEntry(contentTypesXml)?.Open().ToArray();
            }

            Console.WriteLine($"  {refEntries.Count} items found");

            // creating a backup from the target
            string bakFileName = Files.GetNextFileName(dstFile + ".bak", ".")!;
            Console.WriteLine($"Creating backup {Path.GetFileName(bakFileName)}...");
            File.Copy(dstFile, bakFileName);
            Console.WriteLine("  Done");

            Console.WriteLine($"Updating target {Path.GetFileName(dstFile)}...");
            using (ZipArchive dstVsix = ZipFile.Open(dstFile, ZipArchiveMode.Update))
            {
                // removing unnecessary entries
                int removed = 0;
                long size = 0L;
                foreach (ZipArchiveEntry entry in dstVsix.Entries.ToArray())
                {
                    // item found in reference .vsix: skipping
                    if (refEntries.Contains(entry.FullName))
                    {
                        if (!specialEntries.Contains(entry.FullName))
                            size += entry.Length;
                        refEntries.Remove(entry.FullName);
                        continue;
                    }

                    entry.Delete();
                    ++removed;
                }

                if (refEntries.Count > 0)
                    throw new ArgumentException($"Reference package contains files that are not present in target package: {refEntries.Join(", ")}", nameof(refFile));

                Console.WriteLine($"  Removing {removed} entries");

                // taking content type from the reference
                if (contentTypes == null)
                    Console.WriteLine($"  Skipping {contentTypesXml} update because it was not found in reference {Path.GetFileName(refFile)}");
                else
                {
                    Console.WriteLine($"  Writing {contentTypesXml}");
                    dstVsix.GetEntry(contentTypesXml)?.Delete();
                    ZipArchiveEntry entry = dstVsix.CreateEntry(contentTypesXml, CompressionLevel.SmallestSize);
                    using Stream stream = entry.Open();
                    stream.Write(contentTypes, 0, contentTypes.Length);
                }

                // updating catalog.json
                ZipArchiveEntry? catalogEntry = dstVsix.GetEntry(catalogJson);
                if (catalogEntry == null)
                    Console.WriteLine($"  Skipping {catalogJson} because it is not found in target {Path.GetFileName(dstFile)}");
                else
                {
                    JsonObject catalog;
                    using (Stream oldStream = catalogEntry.Open())
                        catalog = JsonObject.Parse(oldStream);
                    JsonArray? packages = catalog["packages"].AsArray;
                    if (packages == null)
                        Console.WriteLine($"  Skipping {catalogJson} due to invalid content: node 'packages' not found");
                    else
                    {
                        bool updated = false;
                        foreach (JsonValue package in packages)
                        {
                            if (package["type"] != "Vsix" || package["installSizes"].AsObject is not JsonObject installSizes)
                                continue;

                            updated = true;
                            installSizes["targetDrive"] = size.ToJson(false);
                            break;
                        }

                        if (updated)
                        {
                            Console.WriteLine($"  Updating {catalogJson}");
                            catalogEntry.Delete();
                            ZipArchiveEntry newEntry = dstVsix.CreateEntry(catalogJson, CompressionLevel.SmallestSize);
                            using Stream newStream = newEntry.Open();
                            catalog.WriteTo(newStream);
                        }
                        else
                            Console.WriteLine($"  Skipping {catalogJson} due to invalid content: node 'installSizes' not found");
                    }
                }

                // updating manifest.json
                ZipArchiveEntry? manifestEntry = dstVsix.GetEntry(manifestJson);
                if (manifestEntry == null)
                    Console.WriteLine($"  Skipping {manifestEntry} because it is not found in target {Path.GetFileName(dstFile)}");
                else
                {
                    Console.WriteLine($"  Updating {manifestJson}");
                    JsonObject manifest;
                    using (Stream oldStream = manifestEntry.Open())
                        manifest = JsonObject.Parse(oldStream);
                    manifest["installSizes"] = new JsonObject { ("targetDrive", size.ToJson(false)) };
                    manifest["files"] = new JsonArray(dstVsix.Entries
                        .Select(e => e.FullName)
                        .Except(specialEntries)
                        .Select(e => (JsonValue)new JsonObject
                        {
                            ("fileName", '/' + e),
                            ("sha256", JsonValue.Null)
                        }));
                    manifestEntry.Delete();
                    ZipArchiveEntry newEntry = dstVsix.CreateEntry(manifestJson, CompressionLevel.SmallestSize);
                    using var newStream = newEntry.Open();
                    manifest.WriteTo(newStream);
                }

                Console.WriteLine("  Saving changes");
            }

            Console.WriteLine($"Target {dstFile} saved successfully.");
        }
    }
}