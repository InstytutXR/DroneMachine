#include "DroneSynth.h"
#include <math.h>

DroneSynth::DroneSynth()
{
	_sampleDuration = 0;
	_mainVolume = 0;
	_osc1Volume = 0;
	_osc2Volume = 0;
	_lfoPhase = 0;
	_lfoPhaseInc = 0;
	_osc1TargetFrequency = -1;
	_osc2TargetFrequency = -1;
}


DroneSynth::~DroneSynth()
{
}

void DroneSynth::Init(double sampleDuration, WavetableSet *wavetableSets, int numWavetableSets)
{
	_sampleDuration = sampleDuration;
	_osc1.Init(sampleDuration, wavetableSets, numWavetableSets);
	_osc2.Init(sampleDuration, wavetableSets, numWavetableSets);
}

void DroneSynth::SetMainVolume(float volume)
{
	_mainVolume = volume;
}

void DroneSynth::SetOsc1Volume(float volume)
{
	_osc1Volume = volume;
}

void DroneSynth::SetOsc1TargetFrequency(double frequency, bool immediate)
{
	if (immediate)
	{
		_osc1.SetFrequency(frequency);
	}
	else
	{
		_osc1TargetFrequency = frequency;
	}
}

void DroneSynth::SetOsc1WavetableAmount(double wtAmt)
{
	_osc1.SetWavetableAmount(wtAmt);
}

void DroneSynth::SetOsc2Volume(float volume)
{
	_osc2Volume = volume;
}

void DroneSynth::SetOsc2TargetFrequency(double frequency, bool immediate)
{
	if (immediate)
	{
		_osc2.SetFrequency(frequency);
	}
	else
	{
		_osc2TargetFrequency = frequency;
	}
}

void DroneSynth::SetOsc2WavetableAmount(double wtAmt)
{
	_osc2.SetWavetableAmount(wtAmt);
}

void DroneSynth::SetLfoFrequency(double frequency)
{
	_lfoPhaseInc = frequency * _sampleDuration;
}

void DroneSynth::Process(float *buffer, int numSamples, int numChannels)
{
	float sample;

	for (int i = 0; i < numSamples; i += numChannels)
	{
		double lfoVolume = fabs(fabs(_lfoPhase - 0.5) - 0.5) * 2;
		sample = (_osc1.GetSample() * _osc1Volume + _osc2.GetSample() * _osc2Volume) * _mainVolume * lfoVolume * lfoVolume * lfoVolume;

		for (int j = 0; j < numChannels; ++j)
		{
			buffer[i + j] *= sample;
		}

		_lfoPhase += _lfoPhaseInc;

		if (_lfoPhase > 1.0)
		{
			while (_lfoPhase > 1.0)
			{
				_lfoPhase -= 1.0;
			}

			if (_osc1TargetFrequency > 0)
			{
				_osc1.SetFrequency(_osc1TargetFrequency);
				_osc1TargetFrequency = -1;
			}

			if (_osc2TargetFrequency > 0)
			{
				_osc2.SetFrequency(_osc2TargetFrequency);
				_osc2TargetFrequency = -1;
			}
		}
	}
}

double DroneSynth::GetLfoPhase()
{
	return _lfoPhase;
}
