using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace korepiLauncher
{
    public partial class Form1 : Form
    {
        private const string apiUrl = "https://api.github.com/repos/Cotton-Buds/calculator-new/releases";
        private readonly HttpClient client;
        private readonly string versionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloaded_version.txt");
        private readonly string accountsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "korepi", "accounts.ini");

        public Form1()
        {
            InitializeComponent();
            client = new HttpClient { DefaultRequestHeaders = { { "User-Agent", "natysubowner" } } };
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            if (!IsUserAdministrator())
            {
                ShowMessage("This application requires administrator privileges to run.", "Error");
                Application.Exit();
                return;
            }

            if (!EnsureFolderExcludedFromDefender())
            {
                ShowMessage("Failed to add the current folder to the Windows Defender exclusion list. Please add it manually.", "Defender Exclusion Failed");
            }

            if (File.Exists(versionFilePath))
            {
                bool isLatestVersion = await IsLatestVersionAsync();

                if (!isLatestVersion)
                {
                    ShowMessage("A new version is available. Please update to the latest version.", "Update Required");
                    //bool isLatestVersion = await IsLatestVersionAsync();
                    //MessageBox.Show($"Is latest version: {isLatestVersion}", "Version Check");
                    await LoadReleasesAsync("gi");
                    return;
                }
            }
            else
            {
                // Se não houver uma versão instalada, devemos carregar as releases
                await LoadReleasesAsync("gi");
            }
        }

        private async Task<bool> IsLatestVersionAsync()
        {
            try
            {
                var response = await client.GetStringAsync(apiUrl);
                var releases = JArray.Parse(response);

                // Encontrar a release mais recente com a tag "gi"
                var latestRelease = releases
                    .OrderByDescending(r => DateTime.Parse(r["published_at"].ToString()))
                    .FirstOrDefault(r => r["tag_name"]?.ToString() == "gi");

                if (latestRelease == null)
                {
                    return false;
                }

                var assets = latestRelease["assets"] as JArray;
                if (assets == null || !assets.Any())
                {
                    return false;
                }

                // Encontrar o ativo mais recente para "NEW_FREE" e "NOT_FREE"
                var latestFreeAsset = assets
                    .Where(a => a["name"]?.ToString()?.Contains("NEW_FREE", StringComparison.OrdinalIgnoreCase) == true)
                    .OrderByDescending(a => DateTime.Parse(a["updated_at"].ToString()))
                    .FirstOrDefault();

                var latestNotFreeAsset = assets
                    .Where(a => a["name"]?.ToString()?.Contains("NOT_FREE", StringComparison.OrdinalIgnoreCase) == true)
                    .OrderByDescending(a => DateTime.Parse(a["updated_at"].ToString()))
                    .FirstOrDefault();

                if (!File.Exists(versionFilePath))
                {
                    return false; // Se o arquivo de versão não existir, deve baixar a nova versão
                }

                string downloadedVersion = File.ReadAllText(versionFilePath).Trim();

                // Determinar qual versão está instalada (gratuita ou paga)
                bool isFreeInstalled = downloadedVersion.Contains("NEW_FREE", StringComparison.OrdinalIgnoreCase);
                bool isNotFreeInstalled = downloadedVersion.Contains("NOT_FREE", StringComparison.OrdinalIgnoreCase);

                string latestVersionAssetName = null;

                if (isFreeInstalled && latestFreeAsset != null)
                {
                    latestVersionAssetName = latestFreeAsset["name"]?.ToString();
                }
                else if (isNotFreeInstalled && latestNotFreeAsset != null)
                {
                    latestVersionAssetName = latestNotFreeAsset["name"]?.ToString();
                }

                if (latestVersionAssetName == null)
                {
                    return false;
                }

                // Normalizar ambas as strings para comparação
                downloadedVersion = NormalizeVersionString(downloadedVersion);
                latestVersionAssetName = NormalizeVersionString(latestVersionAssetName);

                //MessageBox.Show($"Latest version: {latestVersionAssetName}\nDownloaded version: {downloadedVersion}", "Version Info");

                return downloadedVersion.Equals(latestVersionAssetName, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error checking latest version: {ex.Message}", "Error");
            }
            return false;
        }

        private string NormalizeVersionString(string version)
        {
            return version.Replace("_", "").Replace("-", "").Replace(" ", "").ToLower();
        }




        private async Task LoadReleasesAsync(string tag)
        {
            try
            {
                UpdateStatus("Fetching releases...");
                var assets = await GetAssetsByTagAsync(tag);
                if (assets != null && assets.Count > 0)
                {
                    PopulateComboBox(assets);
                    UpdateStatus("Releases loaded!");
                }
                else
                {
                    UpdateStatus("No releases found.");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("Error fetching releases.");
                ShowMessage($"Error: {ex.Message}", "Error");
            }
        }

        private void PopulateComboBox(JArray assets)
        {
            cmbReleases.Items.Clear();
            cmbReleases.Items.Add("Select a version");
            cmbReleases.SelectedIndex = 0;

            foreach (var asset in assets)
            {
                var displayName = asset["name"]?.ToString()?.Contains("NOT_FREE") == true ? "Pertamax" : "Free/Sponsor";
                cmbReleases.Items.Add(displayName);
            }
        }

        private static bool IsUserAdministrator()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static bool EnsureFolderExcludedFromDefender()
        {
            if (IsWindowsDefenderActive() && !IsCurrentFolderExcludedFromDefender())
            {
                return AddCurrentFolderToDefenderExclusions();
            }
            return true;
        }

        private static bool AddFileToDefenderExclusions(string filePath)
        {
            // Adiciona o caminho especificado às exclusões do Windows Defender
            string command = $"Add-MpPreference -ExclusionPath '{filePath}'";
            return string.IsNullOrWhiteSpace(ExecutePowerShellCommand(command));
        }

        private static bool IsFileExcludedFromDefender(string filePath)
        {
            // Comando para listar os caminhos excluídos do Defender
            string command = "Get-MpPreference | Select-Object -ExpandProperty ExclusionPath";
            var excludedPaths = ExecutePowerShellCommand(command)
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return excludedPaths.Any(path => path.Equals(filePath, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsWindowsDefenderActive()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Defender");
                return key?.GetValue("ServiceStart")?.ToString() != "4";
            }
            catch
            {
                return false;
            }
        }

        private static bool IsCurrentFolderExcludedFromDefender()
        {
            return ExecutePowerShellCommand("Get-MpPreference | Select-Object -ExpandProperty ExclusionPath")
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Any(path => path.TrimEnd('\\').Equals(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase));
        }

        private static bool AddCurrentFolderToDefenderExclusions()
        {
            var command = $"Add-MpPreference -ExclusionPath '{AppDomain.CurrentDomain.BaseDirectory}'";
            return string.IsNullOrWhiteSpace(ExecutePowerShellCommand(command));
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool ReleaseCapture();

        private static string ExecutePowerShellCommand(string command)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string errorOutput = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return string.IsNullOrWhiteSpace(errorOutput) ? output : errorOutput;
        }

        private async void BtnDownloadSelected_Click(object sender, EventArgs e)
        {
            if (cmbReleases.SelectedIndex <= 0)
            {
                ShowMessage("Please select a valid version.", "Error");
                return;
            }

            var assets = await GetAssetsByTagAsync("gi");
            if (assets == null || assets.Count < cmbReleases.SelectedIndex)
            {
                ShowMessage("Error: Invalid selection.", "Error");
                return;
            }

            var selectedAsset = assets[cmbReleases.SelectedIndex - 1];
            string selectedVersion = selectedAsset["name"]?.ToString() ?? "";
            string downloadUrl = selectedAsset["browser_download_url"]?.ToString() ?? "";

            if (IsVersionDownloaded(selectedVersion))
            {
                ShowMessage("This version has already been downloaded.", "Notice");
                UpdateStatus("Version already downloaded.");
                return;
            }

            await DownloadUtility.DownloadFileWithProgressAsync(downloadUrl, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedVersion), progressBar, lblStatus, $"Downloading {selectedVersion}");
            ExtractAndMoveFiles(selectedVersion);
        }

        private void ExtractAndMoveFiles(string selectedVersion)
        {
            try
            {
                string downloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedVersion);
                string extractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "korepi");
                string backupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backup", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

                if (Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(backupPath);
                    foreach (var file in Directory.GetFiles(extractPath, "*", SearchOption.AllDirectories))
                    {
                        string relativePath = Path.GetRelativePath(extractPath, file);
                        string backupFilePath = Path.Combine(backupPath, relativePath);
                        Directory.CreateDirectory(Path.GetDirectoryName(backupFilePath));
                        File.Move(file, backupFilePath);
                    }
                }

                ExtractZipFile(downloadPath, extractPath);
                File.Delete(downloadPath);

                MoveFilesToSystem32(Path.Combine(extractPath, "dll"));
                RegisterDownloadedVersion(selectedVersion);
                UpdateStatus("Download, extraction, and file move completed!");
            }
            catch (Exception ex)
            {
                UpdateStatus("Error processing update.");
                ShowMessage($"Error: {ex.Message}", "Error");
            }
        }

        private async void BtnGetHWID_Click(object sender, EventArgs e)
        {
            try
            {
                string hwidExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "korepi", "tools", "get_hwid.exe");

                if (!File.Exists(hwidExePath))
                {
                    UpdateStatus("Downloading get_hwid.exe...");
                    string downloadUrl = await DownloadUtility.GetDownloadUrlAsync(apiUrl, "tools", "get_hwid.exe");
                    if (string.IsNullOrEmpty(downloadUrl))
                    {
                        ShowMessage("Error locating URL for get_hwid.exe.", "Error");
                        return;
                    }

                    // Aguarda o término do download antes de continuar
                    await DownloadUtility.DownloadFileWithProgressAsync(downloadUrl, hwidExePath, progressBar, lblStatus, "Downloading get_hwid.exe...");
                    UpdateStatus("Download of get_hwid.exe completed.");
                }

                // Somente executa o get_hwid.exe após garantir que ele foi baixado com sucesso
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = hwidExePath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false
                };

                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;
                    process.Start();

                    var outputTask = process.StandardOutput.ReadToEndAsync();
                    var errorTask = process.StandardError.ReadToEndAsync();

                    bool processExited = await Task.Run(() => process.WaitForExit(2000));

                    if (!processExited)
                    {
                        SendKeys.SendWait("{ENTER}");
                    }

                    string output = await Task.WhenAny(outputTask, Task.Delay(2000)) == outputTask ? await outputTask : "Output read timeout.";
                    string errorOutput = await Task.WhenAny(errorTask, Task.Delay(2000)) == errorTask ? await errorTask : "Error read timeout.";

                    string hwid = ExtractHWID(output);

                    if (string.IsNullOrWhiteSpace(hwid))
                    {
                        ShowMessage("Failed to extract HWID from output.", "Error");
                        return;
                    }

                    Clipboard.SetText(hwid);
                    ShowMessage("HWID copied to clipboard!", "Success");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error obtaining HWID: {ex.Message}", "Error");
            }
        }

        private static string ExtractHWID(string output)
        {
            string prefix = "hwid:";
            int index = output.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);

            if (index != -1)
            {
                string hwidLine = output.Substring(index + prefix.Length).Trim();
                string[] lines = hwidLine.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                return lines[0].Trim();
            }

            return null;
        }

        private async Task<JArray> GetAssetsByTagAsync(string tagName)
        {
            try
            {
                var response = await client.GetStringAsync(apiUrl);
                var releases = JArray.Parse(response);

                var assets = new JArray();
                foreach (var release in releases.Where(r => r["tag_name"]?.ToString() == tagName).OrderByDescending(r => DateTime.Parse(r["published_at"].ToString())))
                {
                    var filteredAssets = release["assets"]?.Where(a => a["name"]?.ToString()?.Contains("NEW_FREE") == true || a["name"]?.ToString()?.Contains("NOT_FREE") == true)
                                                     .GroupBy(a => a["name"].ToString().Contains("NEW_FREE") ? "NEW_FREE" : "NOT_FREE")
                                                     .Select(g => g.OrderByDescending(a => DateTime.Parse(a["updated_at"].ToString())).FirstOrDefault());
                    foreach (var asset in filteredAssets ?? Enumerable.Empty<JToken>())
                    {
                        assets.Add(asset);
                    }
                }

                return assets;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error fetching assets: {ex.Message}", "Error");
                return null;
            }
        }

        private bool IsVersionDownloaded(string version)
        {
            if (!File.Exists(versionFilePath)) return false;
            var downloadedVersion = File.ReadAllText(versionFilePath).Trim();
            return downloadedVersion == version;
        }

        private void RegisterDownloadedVersion(string version)
        {
            File.WriteAllText(versionFilePath, version);
        }

        private static void ExtractZipFile(string zipPath, string extractPath)
        {
            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }
            ZipFile.ExtractToDirectory(zipPath, extractPath, true);
        }

        private void MoveFilesToSystem32(string sourcePath)
        {
            string targetPath = @"C:\Windows\System32";

            if (Directory.Exists(sourcePath))
            {
                foreach (var filePath in Directory.GetFiles(sourcePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    string destFilePath = Path.Combine(targetPath, fileName);

                    try
                    {
                        // Move o arquivo para System32
                        File.Copy(filePath, destFilePath, true);

                        // Verifica se o Windows Defender está ativo
                        if (IsWindowsDefenderActive())
                        {
                            // Se o arquivo não estiver na lista de exclusão, adiciona-o
                            if (!IsFileExcludedFromDefender(destFilePath))
                            {
                                if (AddFileToDefenderExclusions(destFilePath))
                                {
                                    Console.WriteLine($"{fileName} foi adicionado como exceção no Windows Defender.");
                                }
                                else
                                {
                                    Console.WriteLine($"Falha ao adicionar {fileName} como exceção no Windows Defender.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{fileName} já está na lista de exclusões.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao mover {fileName} para System32: {ex.Message}");
                    }
                }

                // Mover arquivos .exe restantes para a pasta de ferramentas
                string toolsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "korepi", "tools");
                if (!Directory.Exists(toolsPath))
                {
                    Directory.CreateDirectory(toolsPath);
                }

                var exeFiles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "korepi"), "*.exe")
                    .Where(f => !Path.GetFileName(f).StartsWith("v", StringComparison.OrdinalIgnoreCase))
                    .Where(f => !Path.GetFileName(f).StartsWith("injector", StringComparison.OrdinalIgnoreCase));

                foreach (var exeFile in exeFiles)
                {
                    string exeFileName = Path.GetFileName(exeFile);
                    string exeDestPath = Path.Combine(toolsPath, exeFileName);

                    try
                    {
                        File.Move(exeFile, exeDestPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao mover {exeFileName} para {toolsPath}: {ex.Message}");
                    }
                }
            }
        }


        private void UpdateStatus(string message) => lblStatus.Text = message;

        private static void ShowMessage(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LblStatus_Click(object sender, EventArgs e)
        {
            // Placeholder for any future functionality on status label click
        }

        private void ChkLoadAccounts_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLoadAccounts.Checked)
            {
                if (!File.Exists(accountsFilePath))
                {
                    ShowMessage("Before continuing, the file must be generated by korepi.", "Warning");
                    chkLoadAccounts.Checked = false;
                    return;
                }

                cmbAccounts.Enabled = true;
                var accounts = File.ReadAllLines(accountsFilePath)
                    .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("[") && line.Contains('='))
                    .Select(line => line.Split('=')[0].Trim())
                    .ToArray();
                cmbAccounts.Items.Clear();
                cmbAccounts.Items.AddRange(accounts);

                if (cmbAccounts.Items.Count > 0)
                {
                    cmbAccounts.SelectedIndex = 0; // Seleciona automaticamente o primeiro item
                }
            }
            else
            {
                cmbAccounts.Enabled = false;
                cmbAccounts.Items.Clear();
            }
        }

        private void BtnRunKp_Click(object sender, EventArgs e)
        {
            string korepiPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "korepi");
            var exeFile = Directory.GetFiles(korepiPath, "*.exe")
                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Contains("injector", StringComparison.OrdinalIgnoreCase)
                                  || Path.GetFileNameWithoutExtension(f).Contains("v1", StringComparison.OrdinalIgnoreCase));

            if (exeFile != null)
            {
                // Verificar se a checkbox "chkLoadAccounts" está marcada
                if (chkLoadAccounts.Checked)
                {
                    // Se estiver marcada, adicionar um argumento para o processo
                    if (cmbAccounts.SelectedItem == null)
                    {
                        ShowMessage("Please select an account from the dropdown list.", "Error");
                        return;
                    }

                    string selectedAccount = cmbAccounts.SelectedItem.ToString();
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = exeFile,
                        Arguments = $"-account \"{selectedAccount}\"", // Adiciona o argumento da conta selecionada
                        UseShellExecute = true,
                        CreateNoWindow = false
                    });
                }
                else
                {
                    // Se não estiver marcada, apenas executa normalmente
                    Process.Start(exeFile);
                }
            }
            else
            {
                ShowMessage("No executable found in the korepi folder.", "Error");
            }
        }


        private void TitleBar_Paint(object sender, PaintEventArgs e)
        {
            // Placeholder for any future functionality on title bar paint
        }

        private void CmbReleases_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Placeholder for any future functionality on combo box selection change
        }

        public static class DownloadUtility
        {
            private static readonly HttpClient client = new HttpClient { DefaultRequestHeaders = { { "User-Agent", "natysubowner" } } };

            public static async Task DownloadFileWithProgressAsync(string downloadUrl, string outputPath, ProgressBar progressBar, Label statusLabel, string downloadingMessage)
            {
                try
                {
                    UpdateStatus(statusLabel, downloadingMessage);
                    using var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? 1L;
                    using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    using var contentStream = await response.Content.ReadAsStreamAsync();

                    var totalBytesRead = 0L;
                    var buffer = new byte[8192];
                    int bytesRead;

                    progressBar.Value = 0;
                    progressBar.Maximum = 100;

                    while ((bytesRead = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
                    {
                        await fs.WriteAsync(buffer.AsMemory(0, bytesRead));
                        totalBytesRead += bytesRead;
                        int progressPercentage = (int)((double)totalBytesRead / totalBytes * 100);
                        progressBar.Invoke(new Action(() => progressBar.Value = progressPercentage));
                        UpdateStatus(statusLabel, $"Downloading... {progressPercentage}%");
                    }
                    progressBar.Value = 100;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private static void UpdateStatus(Label statusLabel, string message)
            {
                statusLabel.Invoke(new Action(() => statusLabel.Text = message));
            }

            public static async Task<string> GetDownloadUrlAsync(string apiUrl, string tagName, string assetName)
            {
                try
                {
                    var response = await client.GetStringAsync(apiUrl);
                    var releases = JArray.Parse(response);

                    var release = releases.OrderByDescending(r => DateTime.Parse(r["published_at"].ToString())).FirstOrDefault(r => r["tag_name"]?.ToString() == tagName);
                    if (release == null)
                    {
                        return null;
                    }

                    var asset = release["assets"]?.FirstOrDefault(a => a["name"]?.ToString() == assetName);
                    return asset?["browser_download_url"]?.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error getting download URL: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }
    }
}
