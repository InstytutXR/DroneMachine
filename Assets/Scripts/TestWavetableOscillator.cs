using UnityEngine;

namespace DerelictComputer
{
    /// <summary>
    /// Test component for WavetableOscillator
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class TestWavetableOscillator : MonoBehaviour
    {
        /// <summary>
        /// This is just here so you can click the button in editor to play a tone
        /// </summary>
        public bool EditorPlay;

        /// <summary>
        /// Set this to play for some number of seconds
        /// </summary>
        public double Duration = 10;

        /// <summary>
        /// Adjust this to change the volume
        /// </summary>
        [Range(0f, 1f)] public float Volume = 0.25f;

        /// <summary>
        /// Set this to the pitch you want to hear, relative to A440
        /// </summary>
        [Range(-36f, 36f)] public float Pitch = 0;

        /// <summary>
        /// Detunes the oscillators by semitones
        /// </summary>
        [Range(0f, 4f)] public float Detune = 0f;
        
        /// <summary>
        /// Adjust this to morph between waveforms
        /// </summary>
        [Range(0f, 1f)] public double WaveformAmount = 0;


        private WavetableOscillator[] _oscillators;
        private bool _playing;
        private double _playTime;
        private double _sampleRate;

        private void Awake()
        {
            _oscillators = new WavetableOscillator[3];
            for (int i = 0; i < _oscillators.Length; i++)
            {
                _oscillators[i] = new WavetableOscillator();
                _oscillators[i].Init();
            }
            _sampleRate = AudioSettings.outputSampleRate;
        }

        private void Update()
        {
            if (EditorPlay)
            {
                foreach (var oscillator in _oscillators)
                {
                    oscillator.Reset();
                }
                _playTime = AudioSettings.dspTime;
                _playing = true;
                EditorPlay = false;
            }

            float halfLength = (float) _oscillators.Length/2;

            for (int i = 0; i < _oscillators.Length; i++)
            {
                double detuneAmount = Detune*(i - halfLength)/halfLength;
                double frequency = MusicMathUtils.SemitonesToPitch(Pitch+detuneAmount) * 440;
                _oscillators[i].SetFrequency(frequency);
                _oscillators[i].SetVolume(Volume/_oscillators.Length);
                _oscillators[i].SetWaveformAmount(WaveformAmount);

            }
        }

        private void OnAudioFilterRead(float[] buffer, int channels)
        {
            if (!_playing)
            {
                return;
            }

            // check the current time to determine whether we should start playing
            // NOTE: this does not result in sample-accurate playback in the case where
            // the playback would start in the middle of a buffer
            var dspTime = AudioSettings.dspTime;

            if (dspTime < _playTime)
            {
                return;
            }

            // get the buffer from the oscillator
            foreach (var oscillator in _oscillators)
            {
                oscillator.ProcessBuffer(buffer, channels);
            }

            // check the elapsed time to find out if we should stop playing
            // NOTE: this does not result in sample-accurate playback in the case where 
            // the playback would end in the middle of a buffer.
            double elapsed = (dspTime - _playTime) + buffer.Length/(channels*_sampleRate);

            if (elapsed > Duration)
            {
                _playing = false;
            }
        }
    }
}