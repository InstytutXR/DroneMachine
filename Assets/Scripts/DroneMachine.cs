using UnityEngine;

namespace DerelictComputer
{
    public class DroneMachine : MonoBehaviour
    {
        [Range(0.125f, 2f)] public double Frequency;
        public MusicMathUtils.Note RootNote = MusicMathUtils.Note.C;
        public MusicMathUtils.ScaleMode ScaleMode = MusicMathUtils.ScaleMode.Ionian;

        private DroneSynth[] _synths;
        private MusicMathUtils.Note _lastRootNote;
        private MusicMathUtils.ScaleMode _lastScaleMode;

        private void Awake()
        {
            _synths = FindObjectsOfType<DroneSynth>();
        }

        private void Update()
        {
            if (RootNote != _lastRootNote || ScaleMode != _lastScaleMode)
            {
                foreach (var droneSynth in _synths)
                {
                    droneSynth.SetKeyAndScaleMode(RootNote, ScaleMode);
                }

                _lastRootNote = RootNote;
                _lastScaleMode = ScaleMode;
            }

            foreach (var droneSynth in _synths)
            {
                droneSynth.SetLfoFrequency(Frequency);
            }
        }
    }
}
