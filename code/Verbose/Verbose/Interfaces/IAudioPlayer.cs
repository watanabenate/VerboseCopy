namespace Verbose
{
    public interface IAudioPlayer
    {
        bool SetupPlayer();
        void Play();                // Starts Playing audio
        void Stop();                // Stop the audio player
        void Rewind();              // Skip backward a number of seconds
        void Forward();             // Skip forward a number of seconds
        bool IsPlaying { get; }     // returns True if currently playing
        string MediaURL { get; }    // returns the Url of the media currently playing
    }
}
