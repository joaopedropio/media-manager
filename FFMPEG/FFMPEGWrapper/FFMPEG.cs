using FFMPEGWrapper.Arguments;
using Helper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FFMPEGWrapper
{
    public class FFMPEG
    {
        public EventHandler ConvertionDone { get; set; }
        public DataReceivedEventHandler OutputReceived { get; set; }
        public DataReceivedEventHandler ErrorReceived { get; set; }

        private string executable;

        public FFMPEG(string executablePath)
        {
            this.executable = executablePath;
        }

        public async Task<int> Convert(string input, string output, IProfile profile)
        {
            var profileArgs = profile.GetArguments().ToArray();
            return await this.Convert(input, output, profileArgs);
        }

        public async Task<int> Convert(string input, string output, params IArgument[] arguments)
        {
            var inputArg = new InputFilePath(input);
            var outputArg = new OutputFilePath(output);
            var args = ArgumentHelper.CreateArgumentString(inputArg, outputArg, arguments);
            return await Convert(args);
        }

        public async Task<int> Convert(string arguments)
        {
            return await ProcessHelper.Execute(this.executable, arguments, this.ErrorReceived, this.OutputReceived, this.ConvertionDone);
        }
    }
}
