using FFMPEGWrapper.Arguments;
using FFMPEGWrapper.Enums;
using System.Collections.Generic;

namespace FFMPEGWrapper.Profiles
{
    public class SimpleMP4 : IProfile
    {
        private IList<IArgument> arguments;

        public SimpleMP4()
        {
            this.arguments = new List<IArgument>()
            {
                new VideoCodec(VideoCodecEnum.Libx264),
                new AudioCodec(AudioCodecEnum.Aac)
            };
        }

        public IList<IArgument> GetArguments()
        {
            return this.arguments;
        }
    }
}
