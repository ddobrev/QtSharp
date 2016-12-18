using System;
using System.Diagnostics;

namespace QtSharp
{
    public class ProcessHelper
    {
        public static string Run(string path, string args, out string error, bool readOutputByLines = false)
        {
            try
            {
                using (Process process = new Process())
                {
                    Console.WriteLine("Run: " + path);
                    Console.WriteLine("Args: " + args);
                    process.StartInfo.FileName = path;
                    process.StartInfo.Arguments = args;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    var reterror = "";
                    var retout = "";
                    process.OutputDataReceived += (sender, outargs) =>
                    {
                        if (retout != "" && outargs.Data != "") retout += "\r\n";
                        retout += outargs.Data;
                        Console.WriteLine("stdout: {0}", retout);
                    };
                    process.ErrorDataReceived += (sender, errargs) =>
                    {
                        if (reterror != "" && errargs.Data != "") reterror += "\r\n";
                        reterror += errargs.Data;
                        Console.WriteLine("stderr: {0}", reterror);
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    process.CancelOutputRead();
                    process.CancelErrorRead();

                    error = reterror;
                    if (process.ExitCode != 0)
                        throw new Exception("Exit Code is not 0");
                    return readOutputByLines ? string.Empty : retout.Trim().Replace(@"\\", @"\");
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
