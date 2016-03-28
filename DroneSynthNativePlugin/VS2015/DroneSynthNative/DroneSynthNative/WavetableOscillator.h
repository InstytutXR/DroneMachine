#pragma once
#include "WavetableSet.h"

class WavetableOscillator
{
public:
	WavetableOscillator();
	
	~WavetableOscillator();

	void Init(double sampleDuration, WavetableSet *wavetableSets, int numWavetableSets);

	void SetFrequency(double frequency);

	void SetWavetableAmount(double wavetableAmount);

	float GetSample();
private:
	void RefreshWavetables();

	WavetableSet *_wavetableSets;
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

