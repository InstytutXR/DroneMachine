using UnityEditor;
using UnityEngine;

namespace DerelictComputer.DroneMachine
{
    [CustomEditor(typeof(DroneSynth)), CanEditMultipleObjects]
    public class DroneSynthInspector : Editor
    {
        private SerializedProperty _basicMode;       
        private SerializedProperty _lfoCycleMultiplier;       
        private SerializedProperty _scaleInterval;
        private SerializedProperty _octave;
        private SerializedProperty _mainVolume;
        private SerializedProperty _osc1Volume;
        private SerializedProperty _osc2Volume;
        private SerializedProperty _osc1Pitch;
        private SerializedProperty _osc2Pitch;
        private SerializedProperty _osc1Tone;
        private SerializedProperty _osc2Tone;
        private string _newPresetName = "New Preset";

        private void OnEnable()
        {
            _basicMode = serializedObject.FindProperty("_basicMode");
            _lfoCycleMultiplier = serializedObject.FindProperty("_lfoCycleMultiplier");
            _scaleInterval = serializedObject.FindProperty("_scaleInterval");
            _octave = serializedObject.FindProperty("_octave");
            _mainVolume = serializedObject.FindProperty("_mainVolume");
            _osc1Volume = serializedObject.FindProperty("_osc1Volume");
            _osc2Volume = serializedObject.FindProperty("_osc2Volume");
            _osc1Pitch = serializedObject.FindProperty("_osc1Pitch");
            _osc2Pitch = serializedObject.FindProperty("_osc2Pitch");
            _osc1Tone = serializedObject.FindProperty("_osc1Tone");
            _osc2Tone = serializedObject.FindProperty("_osc2Tone");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Musical Settings");

            EditorGUILayout.Slider(_lfoCycleMultiplier, 0.25f, 16f, "Speed Multiplier");

            if (GUI.changed)
            {
                foreach (var o in targets)
                {
                    ((DroneSynth)o).RefreshLfoFrequency();
                }
            }

            EditorGUILayout.IntSlider(_scaleInterval, 1, 7, "Scale Tone");
            EditorGUILayout.IntSlider(_octave, 0, 8, "Octave");

            if (GUI.changed)
            {
                foreach (var o in targets)
                {
                    ((DroneSynth)o).RefreshKeyAndScaleMode();
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Synth Settings");
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Presets");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reload Presets"))
            {
                DroneSynthPresets.Instance.LoadSavedPresets();
            }
            if (GUILayout.Button("Save Presets"))
            {
                DroneSynthPresets.Instance.SavePresets();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            _newPresetName = EditorGUILayout.TextField(_newPresetName);
            if (GUILayout.Button("Add"))
            {
                DroneSynthPresets.Preset preset = new DroneSynthPresets.Preset();
                preset.Name = _newPresetName;
                preset.BasicMode = _basicMode.boolValue;
                preset.MainVolume = _mainVolume.floatValue;
                preset.Osc1Volume = _osc1Volume.floatValue;
                preset.Osc2Volume = _osc2Volume.floatValue;
                preset.Osc1Pitch = _osc1Pitch.doubleValue;
                preset.Osc2Pitch = _osc2Pitch.doubleValue;
                preset.Osc1Tone = _osc1Tone.doubleValue;
                preset.Osc2Tone = _osc2Tone.doubleValue;
                DroneSynthPresets.Instance.AddPreset(preset);

                foreach (var o in targets)
                {
                    DroneSynth ds = (DroneSynth)target;
                    ds.PresetId = preset.Id;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            string[] presetNames = DroneSynthPresets.Instance.GetPresetNames();

            if (presetNames.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();

                DroneSynth ds = (DroneSynth) target;
                DroneSynthPresets.Preset currentPreset = DroneSynthPresets.Instance.GetPresetById(ds.PresetId); 
                int lastPresetIdx = DroneSynthPresets.Instance.GetPresetIndex(currentPreset);

                // fix up invalid preset ids
                if (lastPresetIdx < 0)
                {
                    foreach (var o in targets)
                    {
                        ((DroneSynth) o).PresetId = DroneSynthPresets.Instance.GetPresetByIndex(0).Id;
                    }

                    lastPresetIdx = 0;
                }

                int currentPresetIdx = EditorGUILayout.Popup(lastPresetIdx, presetNames);
                currentPreset = DroneSynthPresets.Instance.GetPresetByIndex(currentPresetIdx);

                if (currentPresetIdx != lastPresetIdx)
                {
                    if (currentPresetIdx == 0)
                    {
                        foreach (var o in targets)
                        {
                            ((DroneSynth)o).PresetId = DroneSynthPresets.Instance.GetPresetByIndex(0).Id;
                        }
                    }
                    else
                    {
                        foreach (var o in targets)
                        {
                            DroneSynth ds1 = (DroneSynth)o;
                            ds1.ApplyPreset(currentPreset);
                            ds1.PresetId = currentPreset.Id;
                        }
                    }
                }

                if (currentPresetIdx != 0)
                {
                    currentPreset.Name = EditorGUILayout.TextField(currentPreset.Name);

                    if (GUILayout.Button("Update Preset"))
                    {
                        currentPreset.BasicMode = _basicMode.boolValue;
                        currentPreset.MainVolume = _mainVolume.floatValue;
                        currentPreset.Osc1Volume = _osc1Volume.floatValue;
                        currentPreset.Osc2Volume = _osc2Volume.floatValue;
                        currentPreset.Osc1Pitch = _osc1Pitch.doubleValue;
                        currentPreset.Osc2Pitch = _osc2Pitch.doubleValue;
                        currentPreset.Osc1Tone = _osc1Tone.doubleValue;
                        currentPreset.Osc2Tone = _osc2Tone.doubleValue;

                        foreach (var synth in FindObjectsOfType<DroneSynth>())
                        {
                            if (synth.PresetId == currentPreset.Id)
                            {
                                synth.ApplyPreset(currentPreset);
                            }
                        }
                    }
                    if (GUILayout.Button("Delete Preset"))
                    {
                        DroneSynthPresets.Instance.DeletePreset(currentPreset.Id);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Mix Settings");
            EditorGUILayout.Slider(_mainVolume, 0f, 1f, "Main Volume");
            EditorGUILayout.Slider(_osc1Volume, 0f, 1f, "Oscillator 1 Volume");
            EditorGUILayout.Slider(_osc2Volume, 0f, 1f, "Oscillator 2 Volume");

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tone Settings");
            _basicMode.boolValue = EditorGUILayout.Toggle("Basic", _basicMode.boolValue);

            if (_basicMode.boolValue)
            {
                float tone = EditorGUILayout.Slider("Tone", (float)_osc1Tone.doubleValue, 0f, 1f);

                if (GUI.changed)
                {
                    _osc1Tone.doubleValue = tone;
                    _osc2Tone.doubleValue = tone;
                }

                float detune = Mathf.Clamp((float)_osc1Pitch.doubleValue, 0f, 12f);
                detune = EditorGUILayout.Slider("Detune", detune, 0f, 12f);

                if (GUI.changed)
                {
                    _osc1Pitch.doubleValue = detune;
                    _osc2Pitch.doubleValue = 0;

                    foreach (var o in targets)
                    {
                        ((DroneSynth)o).RefreshKeyAndScaleMode();
                    }
                }
            }
            else
            {
                EditorGUILayout.Slider(_osc1Tone, 0f, 1f, "Oscillator 1 Tone");
                EditorGUILayout.Slider(_osc2Tone, 0f, 1f, "Oscillator 2 Tone");

                EditorGUILayout.Space();

                EditorGUILayout.Slider(_osc1Pitch, -12f, 12f, "Oscillator 1 Pitch");
                EditorGUILayout.Slider(_osc2Pitch, -12f, 12f, "Oscillator 2 Pitch");

                if (GUI.changed)
                {
                    foreach (var o in targets)
                    {
                        ((DroneSynth)o).RefreshKeyAndScaleMode();
                    }
                }
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
