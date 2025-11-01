using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GiacintFlasher.Lib.Services
{
    internal class ProcessHelper
    {

        //USED CHATGPT
        internal static async Task<string> RunCommandAsync(string fileName, string arguments, int timeoutMs = 1000)
        {
            try
            {
                // Проверка существования файла для Windows
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    //string fullPath = Path.Combine(Environment.CurrentDirectory, $"{fileName}.exe");
                    //if (!File.Exists(LibPlus.FindLib(fileName)))
                    //{
                    //    Debug.Error($"{fileName} not found in current directory: {Environment.CurrentDirectory}");
                    //    return "[ERR] Command not found";
                    //}
                }
                else
                {
                    // Проверка наличия команды в PATH на Linux/Mac
                    var whichPsi = new ProcessStartInfo
                    {
                        FileName = "which",
                        Arguments = fileName,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var whichProc = Process.Start(whichPsi);
                    string whichResult = await whichProc.StandardOutput.ReadToEndAsync();
                    await whichProc.WaitForExitAsync();
                    if (string.IsNullOrWhiteSpace(whichResult))
                    {
                        Debug.Error($"{fileName} not found in PATH");
                        return "[ERR] Command not found";
                    }
                }

                var psi = new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? fileName : "/bin/bash",
                    Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? arguments : $"-c \"{fileName} {arguments}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };

                using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                process.OutputDataReceived += (_, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
                process.ErrorDataReceived += (_, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

                if (!process.Start())
                {
                    Debug.Error($"Failed to start process: {fileName}");
                    return "[ERR] Failed to start process";
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Тайм-аут через Task.WhenAny
                var waitTask = process.WaitForExitAsync();
                var timeoutTask = Task.Delay(timeoutMs);

                if (await Task.WhenAny(waitTask, timeoutTask) != waitTask)
                {
                    try { process.Kill(true); } catch { }
                    Debug.Error($"Process timed out after {timeoutMs} ms: {fileName}");
                    return $"Command not exists in {fileName}";
                }

                int exitCode = process.ExitCode;
                string output = outputBuilder.ToString().Trim();
                string error = errorBuilder.ToString().Trim();

                if (exitCode != 0)
                {
                    Debug.Error($"Process exited with code {exitCode}: {error}");
                    return $"[ERR {exitCode}] {error}";
                }

                if (!string.IsNullOrEmpty(error))
                    Debug.Warning($"Process warning: {error}");

                return output;
            }
            catch (Exception ex)
            {
                Debug.Error($"Exception while running command: {fileName}\n{ex}");
                return $"[EXCEPTION] {ex.Message}";
            }
        }



        internal static async Task Init(string processName, string[] args) => await Init(processName, string.Join(' ', args));

        internal static async Task Init(string processName, string arg)
        {
            var command = arg.Trim();
            string result = await RunCommandAsync(processName, command);
            Debug.Info(result);
        }
    }
}
