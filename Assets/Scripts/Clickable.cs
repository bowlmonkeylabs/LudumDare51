using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts {
    [RequireComponent(typeof(Collider2D))]
    public class Clickable : MonoBehaviour
    {
        [SerializeField] private bool _stop;
        [SerializeField] private UnityEvent _onClick;

        public bool ShouldStop { get { return _stop; } }

        public void EmitClick() {
            _onClick.Invoke();
        }
    }
}
