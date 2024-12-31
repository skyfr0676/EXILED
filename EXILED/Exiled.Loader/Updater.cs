// -----------------------------------------------------------------------
// <copyright file="Updater.cs" company="ExMod Team">
// Copyright (c) ExMod Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    using Exiled.API.Features;
    using Exiled.Loader.GHApi;
    using Exiled.Loader.GHApi.Models;
    using Exiled.Loader.GHApi.Settings;
    using Exiled.Loader.Models;
    using ServerOutput;

#pragma warning disable SA1310 // Field names should not contain underscore

    /// <summary>
    /// A tool to automatically handle updates.
    /// </summary>
    internal sealed class Updater
    {
        private const long REPOID = 833723500;
        private const string INSTALLER_ASSET_NAME_LINUX = "Exiled.Installer-Linux";
        private const string INSTALLER_ASSET_NAME_WIN = "Exiled.Installer-Win.exe";

        private static readonly PlatformID PlatformId = Environment.OSVersion.Platform;
        private static readonly Encoding ProcessEncoding = new UTF8Encoding(false, false);

        private readonly Config config;

        private Updater(Config cfg) => config = cfg;

        private enum Stage
        {
            Free,
            Start,
            Installing,
            Installed,
        }

        /// <summary>
        /// Gets the updater instance.
        /// </summary>
        internal static Updater Instance { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the updater is busy.
        /// </summary>
        internal bool Busy { get; private set; }

        private IEnumerable<ExiledLib> ExiledLib =>
            from Assembly a in AppDomain.CurrentDomain.GetAssemblies()
            let name = a.GetName().Name
            where name.StartsWith("Exiled.", StringComparison.OrdinalIgnoreCase) &&
            !(config.ExcludeAssemblies?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false) &&
            name != Assembly.GetExecutingAssembly().GetName().Name
            select new ExiledLib(a);

        private string Folder => File.Exists($"{PluginAPI.Helpers.Paths.GlobalPlugins.Plugins}/Exiled.Loader.dll") ? "global" : Server.Port.ToString();

        private string InstallerName
        {
            get
            {
                if (PlatformId == PlatformID.Win32NT)
                {
                    return INSTALLER_ASSET_NAME_WIN;
                }
                else if (PlatformId == PlatformID.Unix)
                {
                    return INSTALLER_ASSET_NAME_LINUX;
                }
                else
                {
                    Log.Error("Can't determine your OS platform");
                    Log.Error($"OSDesc: {RuntimeInformation.OSDescription}");
                    Log.Error($"OSArch: {RuntimeInformation.OSArchitecture}");

                    return null;
                }
            }
        }

        /// <summary>
        /// Initializes the updater.
        /// </summary>
        /// <param name="config">The loader config.</param>
        /// <returns>The updater instance.</returns>
        internal static Updater Initialize(Config config)
        {
            if (Instance is not null)
                return Instance;

            Instance = new(config);
            return Instance;
        }

        /// <summary>
        /// Checks for any updates.
        /// </summary>
        internal void CheckUpdate()
        {
            try
            {
                using HttpClient client = CreateHttpClient();
                if (Busy = FindUpdate(client, !PluginAPI.Loader.AssemblyLoader.Dependencies.Exists(x => x.GetName().Name == "Exiled.API"), out NewVersion newVersion))
                    Update(client, newVersion);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        /// <summary>
        /// Creates a HTTP Client, and checks at the ExMod-Team GitHub repository.
        /// </summary>
        /// <returns>Client determining if it was successful connecting to the Exiled GitHub repository.</returns>
        private HttpClient CreateHttpClient()
        {
            HttpClient client = new()
            {
                Timeout = TimeSpan.FromSeconds(480),
            };

            client.DefaultRequestHeaders.Add("User-Agent", $"Exiled.Loader (https://github.com/ExMod-Team/EXILED, {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)})");

            return client;
        }

        /// <summary>
        /// Finds an update using the client.
        /// </summary>
        /// <param name="client"> is the HTTP Client.</param>
        /// <param name="forced"> if the detection was forced.</param>
        /// <param name="newVersion"> if there is a new version of EXILED.</param>
        /// <returns>Returns true if there is an update, otherwise false.</returns>
        private bool FindUpdate(HttpClient client, bool forced, out NewVersion newVersion)
        {
            try
            {
                Thread.Sleep(5000); // Wait for the assemblies to load
                ExiledLib smallestVersion = ExiledLib.Min();

                Log.Info($"Found the smallest version of Exiled - {smallestVersion.Library.GetName().Name}:{smallestVersion.Version}");

                TaggedRelease[] releases = TagReleases(client.GetReleases(REPOID, new GetReleasesSettings(50, 1)).GetAwaiter().GetResult());
                if (FindRelease(releases, out Release targetRelease, smallestVersion, forced))
                {
                    if (!FindAsset(InstallerName, targetRelease, out ReleaseAsset asset))
                    {
                        // Error: no asset
                        Log.Warn("Couldn't find the asset, the update will not be installed");
                    }
                    else
                    {
                        Log.Info($"Found asset - Name: {asset.Name} | Size: {asset.Size} Download: {asset.BrowserDownloadUrl}");
                        newVersion = new NewVersion(targetRelease, asset);
                        return true;
                    }
                }
                else
                {
                    // No errors
                    Log.Info("No new versions found, you're using the most recent version of Exiled!");
                }
            }
            catch (Utf8Json.JsonParsingException)
            {
                Log.Warn("Encountered GitHub ratelimit, unable to check and download the latest version of Exiled.");
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FindUpdate)} threw an exception:\n{ex}");
            }

            newVersion = default;
            return false;
        }

        /// <summary>
        /// Updates the client's version of Exiled.
        /// </summary>
        /// <param name="client"> is the HTTP Client.</param>
        /// <param name="newVersion"> is the updated version of Exiled.</param>
        private void Update(HttpClient client, NewVersion newVersion)
        {
            try
            {
                Log.Info("Downloading installer...");
                using HttpResponseMessage installer = client.GetAsync(newVersion.Asset.BrowserDownloadUrl).ConfigureAwait(false).GetAwaiter().GetResult();
                Log.Info("Downloaded!");

                string serverPath = Environment.CurrentDirectory;
                string installerPath = Path.Combine(serverPath, newVersion.Asset.Name);

                if (File.Exists(installerPath) && (PlatformId == PlatformID.Unix))
                    LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(installerPath);

                using (Stream installerStream = installer.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult())
                using (FileStream fs = new(installerPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    installerStream.CopyToAsync(fs).ConfigureAwait(false).GetAwaiter().GetResult();

                if (PlatformId == PlatformID.Unix)
                    LinuxPermission.SetFileUserAndGroupReadWriteExecutePermissions(installerPath);

                if (!File.Exists(installerPath))
                    Log.Error("Couldn't find the downloaded installer!");

                ProcessStartInfo startInfo = new()
                {
                    WorkingDirectory = serverPath,
                    FileName = installerPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"--exit {(Folder == "global" ? string.Empty : $"--target-port {Folder}")} --target-version {newVersion.Release.TagName} --appdata \"{Paths.AppData}\" --exiled \"{Path.Combine(Paths.Exiled, "..")}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardErrorEncoding = ProcessEncoding,
                    StandardOutputEncoding = ProcessEncoding,
                };

                Process installerProcess = Process.Start(startInfo);
                if (installerProcess is null)
                {
                    Log.Error("Unable to start installer.");
                    Busy = false;
                    return;
                }

                installerProcess.OutputDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        Log.Debug($"[Installer] {args.Data}");
                };
                installerProcess.BeginOutputReadLine();

                installerProcess.ErrorDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        Log.Error($"[Installer] {args.Data}");
                };
                installerProcess.BeginErrorReadLine();

                installerProcess.WaitForExit();

                Log.Info($"Installer exit code: {installerProcess.ExitCode}");

                if (installerProcess.ExitCode == 0)
                {
                    Log.Info("Auto-update complete, server will be restarted the next round!");
                    ServerLogs.AddLog(ServerLogs.Modules.Administrative, "EXILED scheduled server restart after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
                    ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                    ServerConsole.AddOutputEntry(default(ExitActionRestartEntry));
                }
                else
                {
                    Log.Error($"Installer error occured.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(Update)} throw an exception");
                Log.Error(ex);
            }
        }

        /// <summary>
        /// Gets the releases of Exiled.
        /// </summary>
        /// <param name="releases"> gets the array of releases that has been made.</param>
        /// <returns>The last item in the array, which is the newest version of Exiled.</returns>
        private TaggedRelease[] TagReleases(Release[] releases)
        {
            TaggedRelease[] arr = new TaggedRelease[releases.Length];
            for (int z = 0; z < arr.Length; z++)
                arr[z] = new TaggedRelease(releases[z]);

            return arr;
        }

        /// <summary>
        /// Is able to find the release specificed.
        /// </summary>
        /// <param name="releases"> is the list of releases (array).</param>
        /// <param name="release"> is the most recent release of Exiled.</param>
        /// <param name="smallestVersion"> finds the smallest version of the Exiled Library.</param>
        /// <param name="forced"> if this update was forced or not.</param>
        /// <returns>the if the specific release was found or not.</returns>
        private bool FindRelease(TaggedRelease[] releases, out Release release, ExiledLib smallestVersion, bool forced = false)
        {
            bool includePRE = config.ShouldDownloadTestingReleases || ExiledLib.Any(l => l.Version.PreRelease is not null);

            for (int z = 0; z < releases.Length; z++)
            {
                TaggedRelease taggedRelease = releases[z];
                if (taggedRelease.Release.PreRelease && !includePRE)
                    continue;

                if (taggedRelease.Version > smallestVersion.Version || forced)
                {
                    release = taggedRelease.Release;
                    return true;
                }
            }

            release = default;
            return false;
        }

        /// <summary>
        /// Finds the specified asset.
        /// </summary>
        /// <param name="assetName"> passes in the specified asset name.</param>
        /// <param name="release"> passes in the release version.</param>
        /// <param name="asset"> is the asset that is tied to the release.</param>
        /// <returns>if it was able to find the asset or not.</returns>
        private bool FindAsset(string assetName, Release release, out ReleaseAsset asset)
        {
            for (int z = 0; z < release.Assets.Length; z++)
            {
                asset = release.Assets[z];
                if (assetName.Equals(asset.Name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            asset = default;
            return false;
        }
    }
}
