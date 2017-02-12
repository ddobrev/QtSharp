using System;
using System.Diagnostics;
using System.Text;

namespace QtSharp
{
    public class ProcessHelper
    {
        public static string Run(string path, string args, out string error)
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

                var reterror = new StringBuilder();
                var retout = new StringBuilder();
                process.OutputDataReceived += (sender, outargs) =>
                    {
                        if (!string.IsNullOrEmpty(outargs.Data))
                        {
                            if (retout.Length > 0)
                                retout.AppendLine();
                            retout.Append(outargs.Data);
                            Console.WriteLine("stdout: {0}", outargs.Data);
                        }
                    };
                process.ErrorDataReceived += (sender, errargs) =>
                    {
                        if (!string.IsNullOrEmpty(errargs.Data))
                        {
                            if (reterror.Length > 0)
                                reterror.AppendLine();
                            reterror.Append(errargs.Data);
                            Console.WriteLine("stderr: {0}", errargs.Data);
                        }
                    };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                process.CancelOutputRead();
                process.CancelErrorRead();

                error = reterror.ToString();
                if (process.ExitCode != 0)
                    Console.WriteLine($"Error. Process exited with code {process.ExitCode}.");
                return retout.ToString();
            }
        }
    }
}
