#pragma once
#include "Wavetable.h"

#define WAVETABLESET_MAX_WAVETABLES 32

class WavetableSet
{
public:
	WavetableSet();
	
	~WavetableSet();
	
	int AddWavetable(double topFreq, float *samples, int numSamples);

	Wavetable *GetWavetable(int idx);

	int NumWavetables();
private:
	Wavetable _wavetables[WAVETABLESET_MAX_WAVETABLES];
	int _numWavetables;
};

