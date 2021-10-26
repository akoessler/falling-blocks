using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FallingBlocks.Engine.Core.Audio;
using NAudio.Wave;

namespace FallingBlocks.Engine.Windows.Audio
{
    internal class MediaPlayerFactoryWindows : IMediaPlayerFactory, IDisposable
    {
        private readonly ConcurrentDictionary<NAudioMediaPlayer, bool> allMediaPlayers = new ConcurrentDictionary<NAudioMediaPlayer, bool>();

        public void Dispose()
        {
            var playersToDispose = allMediaPlayers.ToList();
            foreach (var player in playersToDispose)
            {
                player.Key?.Dispose();
            }
        }

        public IMediaPlayer CreatePlayerOnce(Stream stream, float volume)
        {
            try
            {
                var player = new NAudioMediaPlayer(stream, volume, false);
                player.Play();
                return player;
            }
            catch
            {
                // ignore sound errors
                return null;
            }
        }

        public IMediaPlayer CreatePlayerLoop(Stream stream, float volume, bool startImmediately = true)
        {
            try
            {
                var player = new NAudioMediaPlayer(stream, volume, true);
                if (startImmediately)
                {
                    player.Play();
                }

                return player;
            }
            catch
            {
                // ignore sound errors
                return null;
            }
        }

        internal void RegisterPlayer(NAudioMediaPlayer newPlayer)
        {
            allMediaPlayers.TryAdd(newPlayer, true);
        }

        internal void RemovePlayer(NAudioMediaPlayer disposedPlayer)
        {
            allMediaPlayers.TryRemove(disposedPlayer, out _);
        }
    }

    internal class NAudioMediaPlayer : IMediaPlayer
    {
        private readonly WaveStream waveProvider;
        private readonly bool loop;

        private WaveOut waveOut;
        private LoopStream loopStream;
        private float volume;

        public NAudioMediaPlayer(Stream stream, float volume, bool loop)
        {
            this.loop = loop;
            this.volume = volume;
            this.waveProvider = new Mp3FileReader(stream);

            this.waveOut = new WaveOut();
            this.waveOut.Volume = this.volume;
            if (this.loop)
            {
                this.loopStream = new LoopStream(this.waveProvider);
                this.waveOut.Init(this.loopStream);
            }
            else
            {
                this.waveOut.Init(this.waveProvider);
            }
        }

        public void Dispose()
        {
            this.waveOut.Stop();
            this.waveOut.Dispose();
            this.loopStream.Dispose();
            this.waveProvider.Dispose();
        }

        public void Play()
        {
            this.waveOut.Play();
        }

        public void Stop()
        {
            this.waveOut.Stop();
        }

        public void ResetPlayback()
        {
            this.waveProvider.Seek(0, SeekOrigin.Begin);
        }

        public void SetVolume(float volume0to1)
        {
            this.volume = Math.Max(0f, Math.Min(1f, volume0to1));
            this.waveOut.Volume = this.volume;
        }
    }

    /// <summary>
    /// Stream for looping playback
    /// </summary>
    internal class LoopStream : WaveStream
    {
        WaveStream sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }

                    // loop
                    sourceStream.Position = 0;
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }
    }
}