using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetDefaultBrowserUI.Services
{
    public class SdbWrapper
    {
        private const string Path = "./Resources/SetDefaultBrowser.exe";

        public SdbWrapper()
        {
            
        }

        private bool IsAppExist()
        {
            return File.Exists(Path);
        }

        public async Task<ExecutionResult<bool>> SetBrowser(string identifier)
        {
            if (!IsAppExist())
            {
                return await Task.FromResult(ExecutionResult<bool>.Fail($"Cannot find {Path}"));
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path,
                    Arguments = identifier,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (output.StartsWith("error:"))
            {
                return await Task.FromResult(ExecutionResult<bool>.Fail(output));
            }
            else
            {
                return await Task.FromResult(ExecutionResult<bool>.Success(true));
            }
        }
    }
}
