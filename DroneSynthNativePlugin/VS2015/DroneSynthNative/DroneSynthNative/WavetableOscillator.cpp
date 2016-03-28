#include "WavetableOscillator.h"



WavetableOscillator::WavetableOscillator()
{
	_numWavetableSets = 0;
	_sampleDuration = 0;
	_phase = 0;
	_phaseInc = 0;
	_wtAmt = 0;
	_wtInterpolate = 0;
	_wtsIdx1 = -1;
	_wtsIdx2 = -1;
	_wtIdx1 = -1;
	_wtIdx2 = -1;
}


WavetableOscillator::~WavetableOscillator()
{
}

void WavetableOscillator::Init(double sampleDuration, WavetableSet *wavetableSets, int numWavetableSets)
{
	_sampleDuration = sampleDuration;
	_wavetableSets = wavetableSets;
	_numWavetableSets = numWavetableSets;
}

void WavetableOscillator::SetFrequency(double frequency)
{
	_phaseInc = frequency * _sampleDuration;
	RefreshWavetables();
}

void WavetableOscillator::SetWavetableAmount(double wtAmt)
{
	_wtAmt = wtAmt;
	RefreshWavetables();
}

void WavetableOscillator::RefreshWavetables()
{
	// reset all wavetable indexes
	_wtsIdx1 = -1;
	_wtsIdx2 = -1;
	_wtIdx1 = -1;
	_wtIdx2 = -1;

	// cache the interpolate amount
	_wtInterpolate = _wtAmt * (_numWavetableSets - 1);
	int wtsIdx = (int)_wtInterpolate;
	_wtInterpolate -= wtsIdx;

	// if we're out of range, don't try to find wavetables
	if (wtsIdx >= _numWavetableSets)
	{
		return;
	}

	// get the first wavetable
	_wtsIdx1 = wtsIdx;

	if (_wavetableSets[_wtsIdx1].NumWavetables() > 0)
	{
		_wtIdx1 = 0;
		while (_phaseInc > _wavetableSets[_wtsIdx1].GetWavetable(_wtIdx1)->topFreq && _wtIdx1 < _wavetableSets[_wtsIdx1].NumWavetables() - 1)
		{
			++_wtIdx1;
		}
	}

	// if we're not at the end of the wavetables, get the second wavetable
	if (++wtsIdx < _numWavetableSets)
	{
		_wtsIdx2 = wtsIdx;

		if (_wavetableSets[_wtsIdx2].NumWavetables() > 0)
		{
			_wtIdx2 = 0;
			while (_phaseInc > _wavetableSets[_wtsIdx2].GetWavetable(_wtIdx2)->topFreq && _wtIdx2 < _wavetableSets[_wtsIdx2].NumWavetables() - 1)
			{
				++_wtIdx2;
			}
		}
	}
}

float WavetableOscillator::GetSample()
{
	// don't do anything if we didn't get a wavetable when we refreshed
	if (_wtsIdx1 == -1 || _wtIdx1 == -1)
	{
		return 0;
	}

	double s0, s1, s2;
	Wavetable *wtTmp;
	double sInterp;
	int sIdx;
	
	// get the sample from the first wavetable set
	{
		wtTmp = _wavetableSets[_wtsIdx1].GetWavetable(_wtIdx1);
		sInterp = _phase * (wtTmp->numSamples - 1);
		sIdx = (int)sInterp;
		sInterp -= sIdx;
		s0 = wtTmp->samples[sIdx];

		// if we're at the end of the table, loop around
		if (++sIdx >= wtTmp->numSamples)
		{
			sIdx = 0;
		}

		s1 = wtTmp->samples[sIdx];

		// linear interpolate
		s2 = s0 + (s1 - s0) * sInterp;
	}

	// if we don't have a second wavetable set, just return the sample from the first wavetable
	if (_wtsIdx2 == -1 || _wtIdx2 == -1)
	{
		return (float)s2;
	}

	// get the sample from the second wavetable set
	{
		wtTmp = _wavetableSets[_wtsIdx2].GetWavetable(_wtIdx2);
		sInterp = _phase * (wtTmp->numSamples - 1);
		sIdx = (int)sInterp;
		sInterp -= sIdx;
		s0 = wtTmp->samples[sIdx];

		// if we're at the end of the table, loop around
		if (++sIdx >= wtTmp->numSamples)
		{
			sIdx = 0;
		}

		s1 = wtTmp->samples[sIdx];

		// linear interpolate
		s1 = s0 + (s1 - s0) * sInterp;
	}

	_phase += _phaseInc;

	while (_phase > 1.0)
	{
		_phase -= 1.0;
	}

	return (float)(s2 + (s1 - s2) * _wtInterpolate);
}
