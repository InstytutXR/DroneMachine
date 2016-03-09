using UnityEngine;

namespace DerelictComputer
{
    public class KeyChanger : MonoBehaviour
    {
        public enum TriggerType
        {
            Awake,
            Start,
            Enable,
            TriggerEnter
        }

        [SerializeField] private TriggerType _triggerType = TriggerType.TriggerEnter;
        [SerializeField, HideInInspector] private MusicMathUtils.Note _rootNote;
        [SerializeField, HideInInspector] private MusicMathUtils.ScaleMode _scaleMode;
        [SerializeField, HideInInspector] private double _frequency = 0.25;
        [SerializeField, HideInInspector] private double _frequencyChangeTime = 0;

        private Collider _collider;

        // properties for editor
        public MusicMathUtils.Note RootNote
        {
            get { return _rootNote; }
            set { _rootNote = value; }
        }

        public MusicMathUtils.ScaleMode ScaleMode
        {
            get { return _scaleMode; }
            set { _scaleMode = value; }
        }

        public float Frequency
        {
            get { return (float)_frequency; }
            set { _frequency = value; }
        }

        public float FrequencyChangeTime
        {
            get { return (float)_frequencyChangeTime; }
            set { _frequencyChangeTime = value; }
        }

        private void Awake()
        {
            if (_triggerType == TriggerType.Awake)
            {
                DoTrigger();
            }
            else if (_triggerType == TriggerType.TriggerEnter)
            {
                _collider = GetComponent<Collider>();

                if (_collider == null)
                {
                    _collider = gameObject.AddComponent<BoxCollider>();
                    _collider.isTrigger = true;
                }
            }
        }

        private void Start()
        {
            if (_triggerType == TriggerType.Start)
            {
                DoTrigger();
            }
        }

        private void OnEnable()
        {
            if (_triggerType == TriggerType.Enable)
            {
                DoTrigger();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggerType == TriggerType.TriggerEnter)
            {
                DoTrigger();
            }
        }

        private void OnDrawGizmos()
        {
            if (_triggerType == TriggerType.TriggerEnter)
            {
                var collider = GetComponent<BoxCollider>();

                if (collider != null)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(collider.center, collider.size);
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(transform.position, Vector3.one);
                }
            }
        }

        private void DoTrigger()
        {
            DroneMachine.Instance.SetKey(_rootNote, _scaleMode);
            DroneMachine.Instance.SetFrequency(_frequency, _frequencyChangeTime);
        }
    }
}
