using System;
using System.Numerics;
using System.Threading.Tasks;
using UIFramework;
using ImGuiNET;
using MapStudio.UI;
using Toolbox.Core;
using Octokit;

namespace MapStudio
{
    /// <summary>
    /// A window that displays update notifications with changelog from GitHub releases.
    /// </summary>
    public class UpdateNotificationWindow : Window
    {
        public override string Name => "Update Available";

        public override ImGuiWindowFlags Flags => ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize;

        private Release LatestRelease;
        private bool IsDownloading = false;
        private bool CheckComplete = false;
        private bool UpdateAvailable = false;
        private string ErrorMessage = "";
        private string CurrentVersion = "";

        // GitHub repository settings
        private const string REPO_OWNER = "OctoSquiddy";
        private const string REPO_NAME = "Splatoon-3-Map-Editor";

        public UpdateNotificationWindow()
        {
            Size = new Vector2(550, 450);
            PlaceAtCenter = true;
            Opened = false;

            // Get current version from Version.txt
            string versionFile = $"{Runtime.ExecutableDir}\\Version.txt";
            if (System.IO.File.Exists(versionFile))
            {
                var lines = System.IO.File.ReadAllLines(versionFile);
                if (lines.Length > 0)
                    CurrentVersion = lines[0];
            }
        }

        /// <summary>
        /// Checks for updates asynchronously and opens the window if an update is available.
        /// </summary>
        public void CheckForUpdatesAsync(bool showIfUpToDate = false)
        {
            Task.Run(() =>
            {
                try
                {
                    UpdaterHelper.Setup(REPO_OWNER, REPO_NAME, "Version.txt", "MapStudio.exe");
                    LatestRelease = UpdaterHelper.TryGetLatest(Runtime.ExecutableDir, 0);

                    UpdateAvailable = LatestRelease != null;
                    CheckComplete = true;
                    ErrorMessage = "";

                    if (UpdateAvailable || showIfUpToDate)
                    {
                        Opened = true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    CheckComplete = true;
                    if (showIfUpToDate)
                        Opened = true;
                }
            });
        }

        public override void Render()
        {
            base.Render();

            // Header with icon
            DrawHeader();

            ImGui.Separator();

            if (!CheckComplete)
            {
                // Loading state
                DrawLoadingState();
            }
            else if (!string.IsNullOrEmpty(ErrorMessage))
            {
                // Error state
                DrawErrorState();
            }
            else if (!UpdateAvailable)
            {
                // Up to date state
                DrawUpToDateState();
            }
            else
            {
                // Update available state
                DrawUpdateAvailableState();
            }

            ImGui.Separator();

            // Footer with buttons
            DrawFooter();
        }

        private void DrawHeader()
        {
            // Icon placeholder
            if (!IconManager.HasIcon("UPDATE_ICON"))
            {
                // Use existing icon if available, otherwise skip
                if (IconManager.HasIcon("TOOL_ICON"))
                {
                    ImGui.Image((IntPtr)IconManager.GetTextureIcon("TOOL_ICON"), new Vector2(48, 48));
                    ImGui.SameLine();
                }
            }

            ImGui.BeginGroup();
            ImGui.SetWindowFontScale(1.4f);

            if (!CheckComplete)
                ImGui.Text("Checking for Updates...");
            else if (!string.IsNullOrEmpty(ErrorMessage))
                ImGui.TextColored(new Vector4(1, 0.3f, 0.3f, 1), "Update Check Failed");
            else if (UpdateAvailable)
                ImGui.TextColored(new Vector4(0.3f, 1, 0.5f, 1), "Update Available!");
            else
                ImGui.TextColored(new Vector4(0.5f, 0.8f, 1, 1), "You're Up To Date!");

            ImGui.SetWindowFontScale(1.0f);
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1), $"Current Version: {CurrentVersion}");
            ImGui.EndGroup();
        }

        private void DrawLoadingState()
        {
            ImGui.Spacing();
            ImGui.Spacing();

            // Animated dots
            string dots = new string('.', (int)(ImGui.GetTime() * 2) % 4);
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - ImGui.CalcTextSize($"Checking for updates{dots}").X) / 2);
            ImGui.Text($"Checking for updates{dots}");

            ImGui.Spacing();
            ImGui.Spacing();
        }

        private void DrawErrorState()
        {
            ImGui.Spacing();
            ImGui.TextColored(new Vector4(1, 0.5f, 0.5f, 1), "Error:");
            ImGui.TextWrapped(ErrorMessage);
            ImGui.Spacing();
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1), "Please check your internet connection and try again.");
            ImGui.Spacing();
        }

        private void DrawUpToDateState()
        {
            ImGui.Spacing();
            ImGui.Spacing();

            // Centered checkmark
            string text = "Your installation is up to date!";
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - ImGui.CalcTextSize(text).X) / 2);
            ImGui.TextColored(new Vector4(0.3f, 1, 0.5f, 1), text);

            ImGui.Spacing();
            ImGui.Spacing();
        }

        private void DrawUpdateAvailableState()
        {
            if (LatestRelease == null)
                return;

            ImGui.Spacing();

            // Release info
            ImGui.TextColored(new Vector4(0.3f, 0.8f, 1, 1), $"New Version: {LatestRelease.TagName}");
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1), $"Released: {LatestRelease.PublishedAt?.ToString("yyyy-MM-dd HH:mm") ?? "Unknown"}");

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            // Changelog header
            ImGui.SetWindowFontScale(1.2f);
            ImGui.TextColored(new Vector4(1, 0.8f, 0.3f, 1), "What's New:");
            ImGui.SetWindowFontScale(1.0f);

            ImGui.Spacing();

            // Changelog content in scrollable area
            float changelogHeight = ImGui.GetWindowHeight() - ImGui.GetCursorPosY() - 80;
            if (ImGui.BeginChild("ChangelogScroll", new Vector2(0, changelogHeight), true))
            {
                if (!string.IsNullOrEmpty(LatestRelease.Body))
                {
                    // Parse and display changelog with formatting
                    DrawFormattedChangelog(LatestRelease.Body);
                }
                else
                {
                    ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1), "No changelog provided for this release.");
                }
            }
            ImGui.EndChild();
        }

        private void DrawFormattedChangelog(string body)
        {
            var lines = body.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                string trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // Headers (## or ###)
                if (trimmed.StartsWith("###"))
                {
                    ImGui.Spacing();
                    ImGui.TextColored(new Vector4(0.8f, 0.6f, 1, 1), trimmed.TrimStart('#').Trim());
                }
                else if (trimmed.StartsWith("##"))
                {
                    ImGui.Spacing();
                    ImGui.SetWindowFontScale(1.1f);
                    ImGui.TextColored(new Vector4(0.3f, 0.8f, 1, 1), trimmed.TrimStart('#').Trim());
                    ImGui.SetWindowFontScale(1.0f);
                }
                // Bullet points
                else if (trimmed.StartsWith("-") || trimmed.StartsWith("*"))
                {
                    string content = trimmed.TrimStart('-', '*', ' ');

                    // Color based on content
                    Vector4 color = new Vector4(1, 1, 1, 1);
                    string icon = "\u2022"; // bullet point

                    string lowerContent = content.ToLower();
                    if (lowerContent.Contains("fix") || lowerContent.Contains("bug"))
                    {
                        color = new Vector4(1, 0.5f, 0.5f, 1);
                        icon = "\uf188"; // bug icon
                    }
                    else if (lowerContent.Contains("add") || lowerContent.Contains("new") || lowerContent.Contains("feature"))
                    {
                        color = new Vector4(0.5f, 1, 0.5f, 1);
                        icon = "\uf055"; // plus icon
                    }
                    else if (lowerContent.Contains("improve") || lowerContent.Contains("update") || lowerContent.Contains("enhance"))
                    {
                        color = new Vector4(0.5f, 0.8f, 1, 1);
                        icon = "\uf118"; // smile icon
                    }

                    ImGui.TextColored(color, $"  {icon}  {content}");
                }
                // Regular text
                else
                {
                    ImGui.TextWrapped(trimmed);
                }
            }
        }

        private void DrawFooter()
        {
            ImGui.Spacing();

            float buttonWidth = 120;
            float totalWidth = UpdateAvailable && !IsDownloading ? buttonWidth * 2 + 10 : buttonWidth;
            ImGui.SetCursorPosX((ImGui.GetWindowWidth() - totalWidth) / 2);

            if (UpdateAvailable && !IsDownloading)
            {
                // Update Now button
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.6f, 0.3f, 1));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.7f, 0.4f, 1));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.15f, 0.5f, 0.25f, 1));

                if (ImGui.Button("Update Now", new Vector2(buttonWidth, 30)))
                {
                    StartUpdate();
                }

                ImGui.PopStyleColor(3);
                ImGui.SameLine();
            }

            if (IsDownloading)
            {
                ImGui.TextColored(new Vector4(0.5f, 0.8f, 1, 1), "Downloading update...");
            }
            else
            {
                // Close / Later button
                string buttonText = UpdateAvailable ? "Later" : "Close";
                if (ImGui.Button(buttonText, new Vector2(buttonWidth, 30)))
                {
                    Opened = false;
                }
            }
        }

        private void StartUpdate()
        {
            if (LatestRelease == null || IsDownloading)
                return;

            IsDownloading = true;
            ProcessLoading.Instance.IsLoading = true;

            Task.Run(() =>
            {
                try
                {
                    UpdaterHelper.DownloadRelease(Runtime.ExecutableDir, LatestRelease, 0, () =>
                    {
                        ProcessLoading.Instance.Update(100, 100, "Update will now install.", "Updater");
                        Console.WriteLine("Installing update..");
                        UpdaterHelper.InstallUpdate("-b");
                        ProcessLoading.Instance.IsLoading = false;
                    });
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    IsDownloading = false;
                    ProcessLoading.Instance.IsLoading = false;
                }
            });
        }
    }
}
