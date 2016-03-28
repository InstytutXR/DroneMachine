#include "DroneSynth.h"
#include "WavetableSet.h"

#define EXPORT_API __declspec(dllexport)

extern "C"
{
	EXPORT_API DroneSynth *DroneSynth_New(double sampleDuration)
	{
		DroneSynth *ds = new DroneSynth();
		ds->Init(sampleDuration);
		return ds;
	}

	EXPORT_API void DroneSynth_Delete(DroneSynth *droneSynth)
	{
		delete droneSynth;
	}

	EXPORT_API int DroneSynth_AddWavetableSet(DroneSynth *droneSynth)
	{
		return droneSynth->AddWavetableSet();
	}

	EXPORT_API void  DroneSynth_AddWavetableToSet(DroneSynth *droneSynth, int wtSetIdx, double topFreq, float *samples, int numSamples)
	{
		droneSynth->AddWavetableToSet(wtSetIdx, topFreq, samples, numSamples);
	}

	EXPORT_API void DroneSynth_SetMainVolume(DroneSynth *droneSynth, float volume)
	{
		droneSynth->SetMainVolume(volume);
	}

	EXPORT_API void DroneSynth_SetOsc1Volume(DroneSynth *droneSynth, float volume)
	{
		droneSynth->SetOsc1Volume(volume);
	}

	EXPORT_API void DroneSynth_SetOsc1TargetFrequency(DroneSynth *droneSynth, double frequency, bool immediate)
	{
		droneSynth->SetOsc1TargetFrequency(frequency, immediate);
	}

	EXPORT_API void DroneSynth_SetOsc1WavetableAmount(DroneSynth *droneSynth, double wtAmt)
	{
		droneSynth->SetOsc1WavetableAmount(wtAmt);
	}

	EXPORT_API void DroneSynth_SetOsc2Volume(DroneSynth *droneSynth, float volume)
	{
		droneSynth->SetOsc2Volume(volume);
	}

	EXPORT_API void DroneSynth_SetOsc2TargetFrequency(DroneSynth *droneSynth, double frequency, bool immediate)
	{
		droneSynth->SetOsc2TargetFrequency(frequency, immediate);
	}

	EXPORT_API void DroneSynth_SetOsc2WavetableAmount(DroneSynth *droneSynth, double wtAmt)
	{
		droneSynth->SetOsc2WavetableAmount(wtAmt);
	}

	EXPORT_API void DroneSynth_SetLfoFrequency(DroneSynth *droneSynth, double frequency)
	{
		droneSynth->SetLfoFrequency(frequency);
	}

	EXPORT_API void DroneSynth_Process(DroneSynth *droneSynth, float* buffer, int numSamples, int numChannels)
	{
		droneSynth->Process(buffer, numSamples, numChannels);
	}

	EXPORT_API double DroneSynth_GetLfoPhase(DroneSynth *droneSynth)
	{
		return droneSynth->GetLfoPhase();
	}
}