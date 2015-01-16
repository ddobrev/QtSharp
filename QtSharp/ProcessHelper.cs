using System;
using System.IO;
using System.Diagnostics;

namespace QtSharp
{
    public class ProcessHelper
    {
        public static string Run(string path, string args, out string error, string wdir="")
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.WorkingDirectory = wdir;
                    process.StartInfo.FileName = path;
                    process.StartInfo.Arguments = args;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.Start();
                    error = process.StandardError.ReadToEnd();
                    if (process.ExitCode != 0)
                    {
                        return string.Empty;
                    }
                    return process.StandardOutput.ReadToEnd().Trim().Replace(@"\\", @"\");
                }
            }
            catch (Exception exception)
            {
                error = string.Format("Calling {0} caused an exception: {1}.", path, exception.Message);
                return string.Empty;
            }
        }
    }
}
