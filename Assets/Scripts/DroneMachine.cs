using UnityEngine;

namespace DerelictComputer
{
    public class DroneMachine : MonoBehaviour
    {
        [SerializeField, Range(0.125f, 2f)] private double _frequency;
        [SerializeField] private MusicMathUtils.Note _rootNote;
        [SerializeField] private MusicMathUtils.ScaleMode _scaleMode;

        private DroneSynth[] _synths;
        private MusicMathUtils.Note _lastRootNote;
        private MusicMathUtils.ScaleMode _lastScaleMode;

        private void Awake()
        {
            _synths = FindObjectsOfType<DroneSynth>();
        }

        private void Update()
        {
            if (_rootNote != _lastRootNote || _scaleMode != _lastScaleMode)
            {
                foreach (var droneSynth in _synths)
                {
                    droneSynth.SetKeyAndScaleMode(_rootNote, _scaleMode, 0.25);
                }

                _lastRootNote = _rootNote;
                _lastScaleMode = _scaleMode;
            }

            foreach (var droneSynth in _synths)
            {
                droneSynth.SetLfoFrequency(_frequency);
            }
        }
    }
}
