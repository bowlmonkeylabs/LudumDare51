using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts {
    [RequireComponent(typeof(Collider2D))]
    public class Droppable : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Draggable> _onDrop;

        public void OnDrop(Draggable draggable) {
            _onDrop.Invoke(draggable);
        }
    }
}
