using UnityEngine;

namespace DerelictComputer
{
    [RequireComponent(typeof(BoxCollider))]
    public class KeyChanger : MonoBehaviour
    {
        [SerializeField] private MusicMathUtils.Note _rootNote;
        [SerializeField] private MusicMathUtils.ScaleMode _scaleMode;

        private void OnTriggerEnter(Collider other)
        {
            var droneMachine = FindObjectOfType<DroneMachine>();
            droneMachine.RootNote = _rootNote;
            droneMachine.ScaleMode = _scaleMode;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
