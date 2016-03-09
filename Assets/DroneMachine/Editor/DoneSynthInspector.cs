using UnityEditor;
using UnityEngine;

namespace DerelictComputer.DroneMachine
{
    [CustomEditor(typeof(DroneSynth)), CanEditMultipleObjects]
    public class DoneSynthInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DroneSynth ds = (DroneSynth) target;

            EditorGUILayout.LabelField("Musical Settings");

            ds.LfoCycleMultiplier = EditorGUILayout.Slider("Speed Multiplier", ds.LfoCycleMultiplier, 0.25f, 16f);
            ds.ScaleInterval = EditorGUILayout.IntSlider("Scale Tone", ds.ScaleInterval, 1, 7);
            ds.Octave = EditorGUILayout.IntSlider("Octave", ds.Octave, 0, 8);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Mix Settings");

            ds.MainVolume = EditorGUILayout.Slider("Volume", ds.MainVolume, 0f, 1f);
            ds.Osc1Volume = EditorGUILayout.Slider("Oscillator 1 Volume", ds.Osc1Volume, 0f, 1f);
            ds.Osc2Volume = EditorGUILayout.Slider("Oscillator 2 Volume", ds.Osc2Volume, 0f, 1f);
            ds.Osc3Volume = EditorGUILayout.Slider("Oscillator 3 Volume", ds.Osc3Volume, 0f, 1f);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tone Settings");
            ds.BasicMode = EditorGUILayout.Toggle("Basic", ds.BasicMode);

            if (ds.BasicMode)
            {

                float tone = EditorGUILayout.Slider("Tone", ds.Osc1Tone, 0f, 1f);

                if (GUI.changed)
                {
                    ds.Osc1Tone = tone;
                    ds.Osc2Tone = tone;
                    ds.Osc3Tone = tone;
                }

                float detune = Mathf.Clamp(ds.Osc1Pitch, 0f, 12f);
                detune = EditorGUILayout.Slider("Detune", detune, 0f, 12f);

                if (GUI.changed)
                {
                    ds.Osc1Pitch = detune;
                    ds.Osc2Pitch = -detune;
                    ds.Osc3Pitch = 0;
                }
            }
            else
            {
                ds.Osc1Tone = EditorGUILayout.Slider("Oscillator 1 Tone", ds.Osc1Tone, 0, 1);
                ds.Osc2Tone = EditorGUILayout.Slider("Oscillator 2 Tone", ds.Osc2Tone, 0, 1);
                ds.Osc3Tone = EditorGUILayout.Slider("Oscillator 3 Tone", ds.Osc3Tone, 0, 1);

                EditorGUILayout.Space();

                ds.Osc1Pitch = EditorGUILayout.Slider("Oscillator 1 Pitch", ds.Osc1Pitch, -12f, 12f);
                ds.Osc2Pitch = EditorGUILayout.Slider("Oscillator 2 Pitch", ds.Osc2Pitch, -12f, 12f);
                ds.Osc3Pitch = EditorGUILayout.Slider("Oscillator 3 Pitch", ds.Osc3Pitch, -12f, 12f);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
