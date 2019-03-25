using Autofac;
using ContentManager;
using MediaManager;

namespace MediaManagerWorker
{
    public static class ApplicationFactory
    {
        public static IContainer Configure()
        {
            var config = new Configuration();
            var builder = new ContainerBuilder();

            builder.RegisterInstance(
                new MediaManager.MediaManager(
                    config.FfmpegExecutablePath,
                    config.MP4BoxExecutablePath,
                    config.SFTPHost,
                    config.SFTPUsername,
                    config.SFTPPassword,
                    config.SFTPPort)).As<IMediaManager>();

            return builder.Build();
        }
    }
}
