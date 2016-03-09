using UnityEditor;
using UnityEngine;

namespace DerelictComputer
{
    [CustomEditor(typeof(KeyChanger)), CanEditMultipleObjects]
    public class KeyChangerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            KeyChanger kc = (KeyChanger) target;

            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("New Key");
            kc.RootNote = (MusicMathUtils.Note)EditorGUILayout.EnumPopup("Root Note", kc.RootNote);
            kc.ScaleMode = (MusicMathUtils.ScaleMode) EditorGUILayout.EnumPopup("Scale Mode", kc.ScaleMode);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("New Frequency/Tempo");
            float freq = kc.Frequency;
            
            freq = EditorGUILayout.Slider("Frequency (Hz)", freq, 0.125f, 8f);
            freq = EditorGUILayout.Slider("Tempo (BPM)", freq*60f, 7.5f, 480f)/60f;

            if (GUI.changed)
            {
                kc.Frequency = freq;
            }

            kc.FrequencyChangeTime = EditorGUILayout.Slider("Slide Duration", kc.FrequencyChangeTime, 0f, 10f);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
