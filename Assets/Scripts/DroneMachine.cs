using UnityEngine;

namespace DerelictComputer
{
    public class DroneMachine : MonoBehaviour
    {
        [SerializeField, Range(0.125f, 2f)] private double _frequency;

        private DroneSynth[] _synths;

        private void Awake()
        {
            _synths = FindObjectsOfType<DroneSynth>();
        }

        private void Update()
        {
            foreach (var droneSynth in _synths)
            {
                droneSynth.SetLfoFrequency(_frequency);
            }
        }
    }
}
