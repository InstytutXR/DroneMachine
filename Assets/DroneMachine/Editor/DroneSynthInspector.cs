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
        private SerializedProperty _osc3Volume;
        private SerializedProperty _osc1Pitch;
        private SerializedProperty _osc2Pitch;
        private SerializedProperty _osc3Pitch;
        private SerializedProperty _osc1Tone;
        private SerializedProperty _osc2Tone;
        private SerializedProperty _osc3Tone;

        private void OnEnable()
        {
            _basicMode = serializedObject.FindProperty("_basicMode");
            _lfoCycleMultiplier = serializedObject.FindProperty("_lfoCycleMultiplier");
            _scaleInterval = serializedObject.FindProperty("_scaleInterval");
            _octave = serializedObject.FindProperty("_octave");
            _mainVolume = serializedObject.FindProperty("_mainVolume");
            _osc1Volume = serializedObject.FindProperty("_osc1Volume");
            _osc2Volume = serializedObject.FindProperty("_osc2Volume");
            _osc3Volume = serializedObject.FindProperty("_osc3Volume");
            _osc1Pitch = serializedObject.FindProperty("_osc1Pitch");
            _osc2Pitch = serializedObject.FindProperty("_osc2Pitch");
            _osc3Pitch = serializedObject.FindProperty("_osc3Pitch");
            _osc1Tone = serializedObject.FindProperty("_osc1Tone");
            _osc2Tone = serializedObject.FindProperty("_osc2Tone");
            _osc3Tone = serializedObject.FindProperty("_osc3Tone");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Musical Settings");
            EditorGUILayout.Slider(_lfoCycleMultiplier, 0.25f, 16f, "Speed Multiplier");
            EditorGUILayout.IntSlider(_scaleInterval, 1, 7, "Scale Tone");
            EditorGUILayout.IntSlider(_octave, 0, 8, "Octave");

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Mix Settings");
            EditorGUILayout.Slider(_mainVolume, 0f, 1f, "Main Volume");
            EditorGUILayout.Slider(_osc1Volume, 0f, 1f, "Oscillator 1 Volume");
            EditorGUILayout.Slider(_osc2Volume, 0f, 1f, "Oscillator 2 Volume");
            EditorGUILayout.Slider(_osc3Volume, 0f, 1f, "Oscillator 3 Volume");

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
                    _osc3Tone.doubleValue = tone;
                }

                float detune = Mathf.Clamp((float)_osc1Pitch.doubleValue, 0f, 12f);
                detune = EditorGUILayout.Slider("Detune", detune, 0f, 12f);

                if (GUI.changed)
                {
                    _osc1Pitch.doubleValue = detune;
                    _osc2Pitch.doubleValue = -detune;
                    _osc3Pitch.doubleValue = 0;
                }
            }
            else
            {
                EditorGUILayout.Slider(_osc1Tone, 0f, 1f, "Oscillator 1 Tone");
                EditorGUILayout.Slider(_osc2Tone, 0f, 1f, "Oscillator 2 Tone");
                EditorGUILayout.Slider(_osc3Tone, 0f, 1f, "Oscillator 3 Tone");

                EditorGUILayout.Space();

                EditorGUILayout.Slider(_osc1Pitch, -12f, 12f, "Oscillator 1 Pitch");
                EditorGUILayout.Slider(_osc2Pitch, -12f, 12f, "Oscillator 2 Pitch");
                EditorGUILayout.Slider(_osc3Pitch, -12f, 12f, "Oscillator 3 Pitch");
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
