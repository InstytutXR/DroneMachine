#pragma once
#include "WavetableOscillator.h"

class DroneSynth
{
public:
	DroneSynth();

	~DroneSynth();

	void Init(double sampleDuration, WavetableSet *wavetableSets, int numWavetableSets);

	void SetMainVolume(float volume);

	void SetOsc1Volume(float volume);

	void SetOsc2Volume(float volume);

	void SetOsc1TargetFrequency(double frequency, bool immediate);

	void SetOsc2TargetFrequency(double frequency, bool immediate);

	void SetOsc1WavetableAmount(double wtAmt);

	void SetOsc2WavetableAmount(double wtAmt);

	void SetLfoFrequency(double frequency);

	void Process(float *buffer, int numSamples, int numChannels);

	double GetLfoPhase();
private:
	WavetableOscillator _osc1;
	WavetableOscillator _osc2;
	double _sampleDuration;
	float _mainVolume;
	float _osc1Volume;
	float _osc2Volume;
	double _lfoPhase;
	double _lfoPhaseInc;
	double _osc1TargetFrequency;
	double _osc2TargetFrequency;
};

