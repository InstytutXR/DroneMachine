#pragma once
#include "WavetableSet.h"

#define WAVETABLEOSCILLATOR_MAX_WAVETABLESETS 8

class WavetableOscillator
{
public:
	WavetableOscillator();
	
	~WavetableOscillator();

	void Init(double sampleDuration);

	int AddWavetableSet();

	void AddWavetableToSet(int wtsIdx, double topFreq, float *samples, int numSamples);

	void SetFrequency(double frequency);

	void SetWavetableAmount(double wavetableAmount);

	float GetSample();
private:
	void RefreshWavetables();

	WavetableSet _wavetableSets[WAVETABLEOSCILLATOR_MAX_WAVETABLESETS];
	int _numWavetableSets;
	double _sampleDuration;
	double _phase;
	double _phaseInc;
	double _wtAmt;
	double _wtInterpolate;
	int _wtsIdx1;
	int _wtsIdx2;
	int _wtIdx1;
	int _wtIdx2;
};

