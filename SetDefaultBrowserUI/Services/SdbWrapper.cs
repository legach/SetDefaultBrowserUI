using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SetDefaultBrowserUI.Models;

namespace SetDefaultBrowserUI.Services
{
    public class SdbWrapper
    {
        private const string Path = "./Resources/SetDefaultBrowser.exe";

        private bool IsAppExist()
        {
            return File.Exists(Path);
        }

        public async Task<ExecutionResult<bool>> SetBrowser(Browser browser)
        {
            if (!IsAppExist())
                return await Task.FromResult(ExecutionResult<bool>.Fail($"Cannot find {Path}"));

            
            var browserName = browser.Identifier.Contains(" ") ? $"\"{browser.Identifier}\"" : browser.Identifier;
            var parameter = $"{browser.Hive} {browserName}";

            var output = await RunProcessAsync(parameter);

            if (output.StartsWith("error:"))
                return await Task.FromResult(ExecutionResult<bool>.Fail(output));

            return await Task.FromResult(ExecutionResult<bool>.Success(true));
        }

        private async Task<string> RunProcessAsync(string parameters = "")
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path,
                    Arguments = parameters,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output ?? string.Empty;
        }

        public async Task<ExecutionResult<List<Browser>>> GetAvailableBrowsers()
        {
            if (!IsAppExist())
                return await Task.FromResult(ExecutionResult<List<Browser>>.Fail($"Cannot find {Path}"));

            var output = await RunProcessAsync();
            
            if (output.StartsWith("error:"))
                return await Task.FromResult(ExecutionResult<List<Browser>>.Fail(output));

            var browserList = new List<Browser>();

            var regexp = new Regex(@"(?<Hive>(HKLM|HKCU))\s(?<Identifier>.*)[\r\n\s]+name:\s(?<Name>.*)[\n\r\s]+path:\s(?<Path>.*)", RegexOptions.IgnoreCase);
            var matches = regexp.Matches(output);
            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var hive = match.Groups["Hive"].Value.Replace("\r","");
                    var identifier = match.Groups["Identifier"].Value.Replace("\r", ""); 
                    var name = match.Groups["Name"].Value.Replace("\r", "");
                    var path = match.Groups["Path"].Value.Replace("\r", "").Replace("\"",""); 

                    var browser = new Browser(name, hive, identifier, path);
                    browserList.Add(browser);
                }
            }
            else
            {
                return await Task.FromResult(ExecutionResult<List<Browser>>.Fail(output));
            }

            return await Task.FromResult(ExecutionResult<List<Browser>>.Success(browserList));
        }
    }
}
