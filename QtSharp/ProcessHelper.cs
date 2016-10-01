using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace QtSharp
{
    public class ProcessHelper
    {
        public static string Run(string path, string args, out string error, bool readOutputByLines = false, bool waitForExit = true)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = path;
                    process.StartInfo.Arguments = args;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.Start();
                    if (waitForExit)
                    {
                        using (var outputWaitHandle = new AutoResetEvent(false))
                        {
                            using (var errorWaitHandle = new AutoResetEvent(false))
                            {
                                var outputBuilder = new StringBuilder();
                                process.OutputDataReceived += (sender, e) =>
                                                              {
                                                                  if (e.Data == null)
                                                                  {
                                                                      outputWaitHandle.Set();
                                                                  }
                                                                  else
                                                                  {
                                                                      outputBuilder.AppendLine(e.Data);
                                                                  }
                                                              };
                                var errorBuilder = new StringBuilder();
                                process.ErrorDataReceived += (sender, e) =>
                                                             {
                                                                 if (e.Data == null)
                                                                 {
                                                                     errorWaitHandle.Set();
                                                                 }
                                                                 else
                                                                 {
                                                                     errorBuilder.AppendLine(e.Data);
                                                                 }
                                                             };
                                process.BeginErrorReadLine();
                                process.BeginOutputReadLine();
                                if (process.WaitForExit(Timeout.Infinite) && outputWaitHandle.WaitOne() &&
                                    errorWaitHandle.WaitOne())
                                {
                                    error = errorBuilder.ToString();
                                    if (process.ExitCode != 0)
                                    {
                                        return string.Empty;
                                    }
                                    return readOutputByLines ? string.Empty : outputBuilder.ToString().Trim().Replace(@"\\", @"\");
                                }
                                error = string.Empty;
                                return string.Empty;
                            }
                        }
                    }
                    while (readOutputByLines && !process.StandardOutput.EndOfStream)
                    {
                        Console.WriteLine(process.StandardOutput.ReadLine());
                    }
                    error = process.StandardError.ReadToEnd();
                    if (process.ExitCode != 0)
                    {
                        return string.Empty;
                    }
                    return readOutputByLines ? string.Empty : process.StandardOutput.ReadToEnd().Trim().Replace(@"\\", @"\");
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
