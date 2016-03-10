using System;
using UnityEngine;

namespace DerelictComputer.DroneMachine
{
    [RequireComponent(typeof(AudioSource))]
    public class DroneSynth : MonoBehaviour
    {
        [SerializeField] private bool _basicMode = true;
        [SerializeField] private double _lfoCycleMultiplier = 1;
        [SerializeField] private int _scaleInterval = 1;
        [SerializeField] private int _octave = 0;
        [SerializeField] private float _mainVolume = 1f;
        [SerializeField] private float _osc1Volume = 0.5f;
        [SerializeField] private float _osc2Volume = 0.5f;
        [SerializeField] private double _osc1Pitch = 0;
        [SerializeField] private double _osc2Pitch = 0;
        [SerializeField] private double _osc1Tone = 0.5;
        [SerializeField] private double _osc2Tone = 0.5;

        private readonly WavetableOscillator _oscillator1 = new WavetableOscillator();
        private readonly WavetableOscillator _oscillator2 = new WavetableOscillator();

        private double _sampleDuration;
        private double _lfoPhaseIncrement;
        private double _targetFrequency;

        public double LfoPhase { get; private set; }

        public void SetLfoFrequency(double frequency)
        {
            _lfoPhaseIncrement = frequency*_sampleDuration/_lfoCycleMultiplier;
        }

        public void SetKeyAndScaleMode(MusicMathUtils.Note rootNote, MusicMathUtils.ScaleMode scaleMode)
        {
            _targetFrequency = MusicMathUtils.ScaleIntervalToFrequency(_scaleInterval, rootNote, scaleMode, _octave);
        }

        private void Start()
        {
            LfoPhase = 0;
            _sampleDuration = 1.0/AudioSettings.outputSampleRate;

            _oscillator1.Init();
            _oscillator1.Reset();

            _oscillator2.Init();
            _oscillator2.Reset();

            // create a dummy clip and start playing it so 3d positioning works
            var dummyClip = AudioClip.Create("dummyclip", 1, 1, AudioSettings.outputSampleRate, false);
            dummyClip.SetData(new float[] { 1 }, 0);
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = dummyClip;
            audioSource.loop = true;
            audioSource.Play();

            DroneMachine.Instance.RegisterDroneSynth(this);

            _oscillator1.SetFrequency(MusicMathUtils.SemitonesToPitch(_osc1Pitch) * _targetFrequency);
            _oscillator2.SetFrequency(MusicMathUtils.SemitonesToPitch(_osc2Pitch) * _targetFrequency);
            _targetFrequency = -1;
        }

        private void Update()
        {
            _oscillator1.SetVolume(_osc1Volume);
            _oscillator1.SetWaveformAmount(_osc1Tone);
            _oscillator2.SetVolume(_osc2Volume);
            _oscillator2.SetWaveformAmount(_osc2Tone);
        }

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            for (int i = 0; i < buffer.Length; i += numChannels)
            {
                if (LfoPhase > 1)
                {
                    LfoPhase -= 1;

                    if (_targetFrequency > 0)
                    {
                        _oscillator1.SetFrequency(MusicMathUtils.SemitonesToPitch(_osc1Pitch) * _targetFrequency);
                        _oscillator2.SetFrequency(MusicMathUtils.SemitonesToPitch(_osc2Pitch) * _targetFrequency);
                        _targetFrequency = -1;
                    }

                    for (int j = 0; j < numChannels; j++)
                    {
                        buffer[i + j] = 0;
                    }
                }
                else
                {
                    float sample = 0;

                    _oscillator1.GetSampleAndUpdatePhase(ref sample, false);
                    _oscillator2.GetSampleAndUpdatePhase(ref sample);

                    double lfoVolume = Math.Abs(Math.Abs(LfoPhase - 0.5) - 0.5) * 2;
                    sample *= _mainVolume*(float) (lfoVolume*lfoVolume*lfoVolume);

                    for (int j = 0; j < numChannels; j++)
                    {
                        buffer[i + j] *= sample;
                    }
                }

                LfoPhase += _lfoPhaseIncrement;
            }
        }
    }
}
