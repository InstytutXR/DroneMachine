#include "WavetableSet.h"

WavetableSet::WavetableSet()
{
	_numWavetables = 0;

	for (int i = 0; i < WAVETABLESET_MAX_WAVETABLES; i++)
	{
		_wavetables[i].topFreq = 0;
		_wavetables[i].samples = 0;
		_wavetables[i].numSamples = 0;
	}
}


WavetableSet::~WavetableSet()
{
	for (int i = 0; i < WAVETABLESET_MAX_WAVETABLES; i++)
	{
		float *tmp = _wavetables[i].samples;

		if (tmp != 0)
		{
			delete[] tmp;
		}
	}
}

int WavetableSet::AddWavetable(double topFreq, float *samples, int numSamples)
{
	if (_numWavetables >= WAVETABLESET_MAX_WAVETABLES)
	{
		return _numWavetables;
	}

	_wavetables[_numWavetables].topFreq = topFreq;
	_wavetables[_numWavetables].numSamples = numSamples;
	
	float *s = _wavetables[_numWavetables].samples = new float[numSamples];

	for (int i = 0; i < numSamples; i++)
	{
		s[i] = samples[i];
	}

	++_numWavetables;

	return 0;
}

Wavetable *WavetableSet::GetWavetable(int idx)
{
	if (idx < 0 || idx >= _numWavetables)
	{
		return 0;
	}

	return &_wavetables[idx];
}

int WavetableSet::NumWavetables()
{
	return _numWavetables;
}
