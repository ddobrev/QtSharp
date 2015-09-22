using System;
using System.IO;

namespace QtSharp.CLI
{
    public class ConsoleLogger
    {

        /// <summary> Original Console Output Stream. </summary>
        /// <value> Original Console Output Stream. </value>
        public TextWriter ConsoleStdOutput { get; set; }

        /// <summary> Original Console Output Stream. </summary>
        /// <value> Original Console Output Stream. </value>
        public TextWriter ConsoleErrOutput { get; set; }

        /// <summary> Path to the Log File. </summary>
        /// <value> Path to the Log File. </value>
        public string LogFilePath
        {
            get
            {
                return logFilePath;
            }
        }
        protected string logFilePath;

        /// <summary> Filestream used for output. </summary>
        protected FileStream fstream { get; set; }

        /// <summary> Filestream Writer used for output. </summary>
        protected StreamWriter fstreamwriter { get; set; }

        /// <summary> Default constructor. </summary>
        public ConsoleLogger()
        {
            ConsoleStdOutput = Console.Out;
            ConsoleErrOutput = Console.Error;
            logFilePath = null;
        }

        /// <summary> Sets the log file output. </summary>
        /// <param name="filename"> Filename of the log file to use. </param>
        public void SetLogFile(string filename)
        {
            logFilePath = Path.Combine("logs", filename);
        }

        /// <summary> Starts console redirection. </summary>
        public void Start()
        {
            Stop();
            fstream = new FileStream(logFilePath, FileMode.Create);
            fstreamwriter = new StreamWriter(fstream);
            Console.SetOut(fstreamwriter);
            Console.SetError(fstreamwriter);
        }

        /// <summary> Stops console redirection. </summary>
        public void Stop()
        {
            Console.SetOut(ConsoleStdOutput);
            Console.SetError(ConsoleErrOutput);
            if (fstreamwriter != null) fstreamwriter.Close();
            if (fstream != null) fstream.Close();
        }

        /// <summary> Creates log directory. </summary>
        public void CreateLogDirectory()
        {
            if (Directory.Exists("logs") == false) Directory.CreateDirectory("logs");
        }
    }
}
