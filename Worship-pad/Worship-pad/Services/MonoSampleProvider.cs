using NAudio.Wave;

namespace WorshipPad.Core.Services;

public class MonoSampleProvider : ISampleProvider
{
    private readonly ISampleProvider _source;

    public WaveFormat WaveFormat { get; }

    public MonoSampleProvider(
        ISampleProvider source)
    {
        _source = source;

        WaveFormat =
            WaveFormat.CreateIeeeFloatWaveFormat(
                source.WaveFormat.SampleRate,
                1);
    }

    public int Read(
        float[] buffer,
        int offset,
        int count)
    {
        int sourceChannels =
            _source.WaveFormat.Channels;

        if (sourceChannels == 1)
        {
            return _source.Read(
                buffer,
                offset,
                count);
        }

        int sourceSamplesNeeded =
            count * sourceChannels;

        float[] sourceBuffer =
            new float[sourceSamplesNeeded];

        int samplesRead =
            _source.Read(
                sourceBuffer,
                0,
                sourceSamplesNeeded);

        int framesRead =
            samplesRead / sourceChannels;

        for (int frame = 0;
             frame < framesRead;
             frame++)
        {
            float sum = 0;

            for (int channel = 0;
                 channel < sourceChannels;
                 channel++)
            {
                sum += sourceBuffer[
                    frame * sourceChannels + channel];
            }

            buffer[offset + frame] =
                sum / sourceChannels;
        }

        return framesRead;
    }
}