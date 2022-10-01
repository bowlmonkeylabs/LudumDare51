using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BML.Scripts {
    [RequireComponent(typeof(Collider2D))]
    public class Draggable : MonoBehaviour
    {
        [SerializeField] private LayerMask _droppableLayerMask;
        [SerializeField] private UnityEvent _onStartDrag;
        [SerializeField] private UnityEvent _onEndDrag;

        private bool _dragging = false;
        private Vector3 _dragOffset;
        private Vector3 _dragStart;

        public void StartDrag() {
            _dragOffset = transform.position - getMouseWorldPositionOnTransformLevel();

            _dragStart = transform.position;

            _dragging = true;
            _onStartDrag.Invoke();
        }

        public void EndDrag() {
            _dragging = false;
            _onEndDrag.Invoke();

            Ray clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(clickRay.origin, clickRay.direction, Mathf.Infinity, _droppableLayerMask);
            
            Droppable droppable = raycastHit ? raycastHit.transform.GetComponent<Droppable>() : null;
            if(droppable != null) {
                droppable.OnDrop(this);
            } else {
                transform.position = _dragStart;
            }
        }

        void Update() {
            if(_dragging) {
                transform.position = getMouseWorldPositionOnTransformLevel() + _dragOffset;
            }
        }

        private Vector3 getMouseWorldPositionOnTransformLevel() {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            pos.z = transform.position.z;
            return pos;
        }
    }
}
