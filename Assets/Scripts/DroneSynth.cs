using System;
using UnityEngine;

namespace DerelictComputer
{
    [RequireComponent(typeof(AudioSource))]
    public class DroneSynth : MonoBehaviour
    {
        [SerializeField, Range(0.125f, 16f)] private double _lfoCycleMultiplier = 1;
        [SerializeField, Range(1, 7)] private int _scaleInterval = 1;
        [SerializeField, Range(0, 8)] private int _octave = 0;
        [SerializeField, Range(0f, 1f)] private float _mainVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float _osc1Volume = 0.33f;
        [SerializeField, Range(0f, 1f)] private float _osc2Volume = 0.33f;
        [SerializeField, Range(0f, 1f)] private float _osc3Volume = 0.33f;
        [SerializeField, Range(-12f, 12f)] private double _osc1Pitch = -0.5;
        [SerializeField, Range(-12f, 12f)] private double _osc2Pitch = 0;
        [SerializeField, Range(-12f, 12f)] private double _osc3Pitch = 0.5;
        [SerializeField, Range(0f, 1f)] private double _osc1Tone = 0;
        [SerializeField, Range(0f, 1f)] private double _osc2Tone = 0.5;
        [SerializeField, Range(0f, 1f)] private double _osc3Tone = 1;

        private readonly WavetableOscillator _oscillator1 = new WavetableOscillator();
        private readonly WavetableOscillator _oscillator2 = new WavetableOscillator();
        private readonly WavetableOscillator _oscillator3 = new WavetableOscillator();

        private double _sampleDuration;
        private double _lfoPhaseIncrement;
        private double _lfoPhase;
        private double _targetFrequency;

        public void SetLfoFrequency(double frequency)
        {
            _lfoPhaseIncrement = frequency*_sampleDuration/_lfoCycleMultiplier;
        }

        public void SetKeyAndScaleMode(MusicMathUtils.Note rootNote, MusicMathUtils.ScaleMode scaleMode)
        {
            _targetFrequency = MusicMathUtils.ScaleIntervalToFrequency(_scaleInterval, rootNote, scaleMode, _octave);
        }

        private void Awake()
        {
            _lfoPhase = 0;
            _sampleDuration = 1.0/AudioSettings.outputSampleRate;

            _oscillator1.Init();
            _oscillator1.Reset();

            _oscillator2.Init();
            _oscillator2.Reset();

            _oscillator3.Init();
            _oscillator3.Reset();

            // create a dummy clip and start playing it so 3d positioning works
            var dummyClip = AudioClip.Create("dummyclip", 1, 1, AudioSettings.outputSampleRate, false);
            dummyClip.SetData(new float[] { 1 }, 0);
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = dummyClip;
            audioSource.loop = true;
            audioSource.Play();

            DroneMachine.Instance.RegisterDroneSynth(this);
        }

        private void Update()
        {
            _oscillator1.SetVolume(_osc1Volume);
            _oscillator1.SetWaveformAmount(_osc1Tone);
            _oscillator2.SetVolume(_osc2Volume);
            _oscillator2.SetWaveformAmount(_osc2Tone);
            _oscillator3.SetVolume(_osc3Volume);
            _oscillator3.SetWaveformAmount(_osc3Tone);
        }

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            float[] tmpBuffer = new float[buffer.Length];

            for (int i = 0; i < buffer.Length; i++)
            {
                tmpBuffer[i] = 0;
            }

            _oscillator1.ProcessBuffer(tmpBuffer, numChannels);
            _oscillator2.ProcessBuffer(tmpBuffer, numChannels);
            _oscillator3.ProcessBuffer(tmpBuffer, numChannels);

            for (int i = 0; i < buffer.Length; i++)
            {
                // get LFO volume
                double lfoVolume = Math.Pow(Math.Abs(Math.Abs(_lfoPhase - 0.5) - 0.5)*2, 4);

                _lfoPhase += _lfoPhaseIncrement;

                if (_lfoPhase > 1)
                {
                    _lfoPhase -= 1;

                    if (_targetFrequency > 0)
                    {
                        _oscillator1.SetFrequency(MusicMathUtils.SemitonesToPitch(_osc1Pitch) * _targetFrequency);
                        _oscillator2.SetFrequency(MusicMathUtils.SemitonesToPitch(_osc2Pitch) * _targetFrequency);
                        _oscillator3.SetFrequency(MusicMathUtils.SemitonesToPitch(_osc3Pitch) * _targetFrequency);
                        _targetFrequency = -1;
                    }
                }

                buffer[i] *= tmpBuffer[i] * _mainVolume * (float)lfoVolume;
            }
        }
    }
}
