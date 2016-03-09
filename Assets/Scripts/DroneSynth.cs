using System;
using UnityEngine;

namespace DerelictComputer
{
    [RequireComponent(typeof(AudioSource))]
    public class DroneSynth : MonoBehaviour
    {
        [SerializeField, HideInInspector] private bool _basicMode = true;
        [SerializeField, HideInInspector] private double _lfoCycleMultiplier = 1;
        [SerializeField, HideInInspector] private int _scaleInterval = 1;
        [SerializeField, HideInInspector] private int _octave = 0;
        [SerializeField, HideInInspector] private float _mainVolume = 1f;
        [SerializeField, HideInInspector] private float _osc1Volume = 0.33f;
        [SerializeField, HideInInspector] private float _osc2Volume = 0.33f;
        [SerializeField, HideInInspector] private float _osc3Volume = 0.33f;
        [SerializeField, HideInInspector] private double _osc1Pitch = -0.5;
        [SerializeField, HideInInspector] private double _osc2Pitch = 0;
        [SerializeField, HideInInspector] private double _osc3Pitch = 0.5;
        [SerializeField, HideInInspector] private double _osc1Tone = 0;
        [SerializeField, HideInInspector] private double _osc2Tone = 0.5;
        [SerializeField, HideInInspector] private double _osc3Tone = 1;

        private readonly WavetableOscillator _oscillator1 = new WavetableOscillator();
        private readonly WavetableOscillator _oscillator2 = new WavetableOscillator();
        private readonly WavetableOscillator _oscillator3 = new WavetableOscillator();

        private double _sampleDuration;
        private double _lfoPhaseIncrement;
        private double _lfoPhase;
        private double _targetFrequency;

#if UNITY_EDITOR
        // editor properties
        public bool BasicMode
        {
            get { return _basicMode; }
            set { _basicMode = value; }
        }

        public float LfoCycleMultiplier
        {
            get { return (float)_lfoCycleMultiplier; }
            set { _lfoCycleMultiplier = value; }
        }

        public int ScaleInterval
        {
            get { return _scaleInterval; }
            set { _scaleInterval = value; }
        }

        public int Octave
        {
            get { return _octave; }
            set { _octave = value; }
        }

        public float MainVolume
        {
            get { return _mainVolume; }
            set { _mainVolume = value; }
        }

        public float Osc1Volume
        {
            get { return _osc1Volume; }
            set { _osc1Volume = value; }
        }

        public float Osc2Volume
        {
            get { return _osc2Volume; }
            set { _osc2Volume = value; }
        }

        public float Osc3Volume
        {
            get { return _osc3Volume; }
            set { _osc3Volume = value; }
        }

        public float Osc1Pitch
        {
            get { return (float)_osc1Pitch; }
            set { _osc1Pitch = value; }
        }

        public float Osc2Pitch
        {
            get { return (float)_osc2Pitch; }
            set { _osc2Pitch = value; }
        }

        public float Osc3Pitch
        {
            get { return (float) _osc3Pitch; }
            set { _osc3Pitch = value; }
        }

        public float Osc1Tone
        {
            get { return (float)_osc1Tone; }
            set { _osc1Tone = value; }
        }

        public float Osc2Tone
        {
            get { return (float) _osc2Tone; }
            set { _osc2Tone = value; }
        }

        public float Osc3Tone
        {
            get { return (float) _osc3Tone; }
            set { _osc3Tone = value; }
        }
#endif

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
