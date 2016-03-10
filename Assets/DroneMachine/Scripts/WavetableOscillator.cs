#define INTERPOLATE_SAMPLES
#define INTERPOLATE_WAVETABLES

using UnityEngine;

namespace DerelictComputer.DroneMachine
{
    /// <summary>
    /// Oscillator that reads from a collection of waveforms and can morph between them
    /// 
    /// Based on http://www.earlevel.com/main/2012/05/25/a-wavetable-oscillator%E2%80%94the-code/
    /// </summary>
    public class WavetableOscillator
    {
        private WavetableSet[] _wavetableSets;
        private double _waveformAmount;
        private double _phase;
        private double _phaseIncrement;
        private double _sampleDuration;
        private float _volume;

        public WavetableOscillator()
        {
            _phase = 0.0;
            _phaseIncrement = 0;
            _waveformAmount = 0f;
        }

        public void Init()
        {
            _sampleDuration = 1.0 / AudioSettings.outputSampleRate;
            _wavetableSets = WavetableSet.GetWavetableSets();
        }

        public void Reset()
        {
            _phase = 0.0;
        }

        public void SetWaveformAmount(double waveformAmount)
        {
            _waveformAmount = waveformAmount;
        }

        public void SetFrequency(double frequency)
        {
            _phaseIncrement = frequency * _sampleDuration;
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
        }

        public void GetSampleAndUpdatePhase(ref float sample, bool additive = true)
        {
            if (additive)
            {
                sample += GetSample()*_volume;
            }
            else
            {
                sample = GetSample() * _volume;
            }

            UpdatePhase();
        }

        private void UpdatePhase()
        {
            _phase += _phaseIncrement;

            if (_phase > 1.0)
            {
                _phase -= 1.0;
            }
        }

        private float GetSample()
        {
#if INTERPOLATE_WAVETABLES
            // get the first wavetable index and the interpolation amount
            var wtsIdxFrac = _waveformAmount*(_wavetableSets.Length - 1);
            var wtsIdxInt = (int) wtsIdxFrac;
            wtsIdxFrac -= wtsIdxInt;

            // grab the wavetable from the first set
            var wt1 = GetWavetable(_wavetableSets[wtsIdxInt]);

            // if we're at the end of the set of wavetables, don't bother interpolating
            if (++wtsIdxInt >= _wavetableSets.Length)
            {
                return GetSampleFromWavetable(wt1);
            }

            // grab the wavetable from the next set
            var wt2 = GetWavetable(_wavetableSets[wtsIdxInt]);

            // interpolate between both wavetables' samples
            var s0 = GetSampleFromWavetable(wt1);
            var s1 = GetSampleFromWavetable(wt2);
            return (float) (s0 + (s1 - s0)*wtsIdxFrac);
#else
            var wt = GetWavetable(_wavetableSets[(int)(_waveformAmount * (_wavetableSets.Length - 1))]);
            return GetSampleFromWavetable(wt);
#endif
        }

        private WavetableSet.Wavetable GetWavetable(WavetableSet wavetableSet)
        {
            int wtIdx = 0;
            while (_phaseIncrement >= wavetableSet.Wavetables[wtIdx].TopFrequency && wtIdx < wavetableSet.Wavetables.Length - 1)
            {
                ++wtIdx;
            }
            return wavetableSet.Wavetables[wtIdx];
        }

        private float GetSampleFromWavetable(WavetableSet.Wavetable wavetable)
        {
#if INTERPOLATE_SAMPLES
            // get the first sample index and the amount of interpolation
            var sIdxFrac = _phase * (wavetable.Table.Length - 1);
            var sIdxInt = (int)sIdxFrac;
            sIdxFrac -= sIdxInt;
            var s0 = wavetable.Table[sIdxInt];

            // if we're at the end of the table, don't bother interpolating
            if (++sIdxInt >= wavetable.Table.Length)
            {
                sIdxInt = 0;
            }

            var s1 = wavetable.Table[sIdxInt];

            // interpolate between samples
            return (float)(s0 + (s1 - s0) * sIdxFrac);
#else
            // truncate
            return wavetable.Table[(int)(_phase * wavetable.Table.Length)];
#endif
        }
    }
}
