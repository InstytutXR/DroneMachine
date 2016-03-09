using UnityEngine;

namespace DerelictComputer
{
    public class TestKeyStuff : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log(MusicMathUtils.ScaleIntervalToFrequency(1, MusicMathUtils.Note.C, MusicMathUtils.ScaleMode.Ionian, 1));
            Debug.Log(MusicMathUtils.ScaleIntervalToFrequency(7, MusicMathUtils.Note.D, MusicMathUtils.ScaleMode.Dorian, 0));
            Debug.Log(MusicMathUtils.ScaleIntervalToFrequency(6, MusicMathUtils.Note.E, MusicMathUtils.ScaleMode.Phrygian, 0));
            Debug.Log(MusicMathUtils.ScaleIntervalToFrequency(5, MusicMathUtils.Note.F, MusicMathUtils.ScaleMode.Lydian, 0));
            Debug.Log(MusicMathUtils.ScaleIntervalToFrequency(4, MusicMathUtils.Note.G, MusicMathUtils.ScaleMode.Mixolydian, 0));
            Debug.Log(MusicMathUtils.ScaleIntervalToFrequency(3, MusicMathUtils.Note.A, MusicMathUtils.ScaleMode.Aeolian, 0));
            Debug.Log(MusicMathUtils.ScaleIntervalToFrequency(2, MusicMathUtils.Note.B, MusicMathUtils.ScaleMode.Locrian, 0));
        }
    }
}
