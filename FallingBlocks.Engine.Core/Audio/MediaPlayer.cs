using System;
using System.IO;

namespace FallingBlocks.Engine.Core.Audio
{
    public static class MediaPlayer
    {
        public static IMediaPlayerFactory Factory { get; set; } = new EmptyMediaPlayerFactory();

        public static void Shutdown()
        {
            var oldFactory = Factory;
            Factory = new EmptyMediaPlayerFactory();
            oldFactory.Dispose();
        }
    }

    public interface IMediaPlayerFactory : IDisposable
    {
        IMediaPlayer CreatePlayerOnce(Stream stream, float volume);

        IMediaPlayer CreatePlayerLoop(Stream stream, float volume, bool startImmediately = true);
    }

    public interface IMediaPlayer : IDisposable
    {
        void Play();
        void Stop();
        void ResetPlayback();
        void SetVolume(float volume0to1);
    }

    internal class EmptyMediaPlayerFactory : IMediaPlayerFactory
    {
        public IMediaPlayer CreatePlayerOnce(Stream stream, float volume)
        {
            return null;
        }

        public IMediaPlayer CreatePlayerLoop(Stream stream, float volume, bool startImmediately = true)
        {
            return null;
        }

        public void Dispose()
        {
        }
    }
}