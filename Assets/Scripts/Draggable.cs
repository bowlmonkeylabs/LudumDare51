using System;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.Variables;
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
        [SerializeField] private Vector3Variable _mouseWorldPosInMinigame;
        [SerializeField] private GameEvent _onCancelDrag;

        private bool _dragging = false;
        private Vector3 _dragOffset;
        private Vector3 _dragStart;

        private void OnEnable()
        {
            _onCancelDrag.Subscribe(EndDrag);
        }

        private void OnDisable()
        {
            _onCancelDrag.Unsubscribe(EndDrag);
        }

        public void StartDrag() {
            _dragOffset = transform.position - getMouseWorldPositionOnTransformLevel();

            _dragStart = transform.position;

            _dragging = true;
            _onStartDrag.Invoke();
        }

        public void EndDrag()
        {
            if (!_dragging)
                return;
            
            _dragging = false;
            _onEndDrag.Invoke();

            RaycastHit2D raycastHit = Physics2D.Raycast(_mouseWorldPosInMinigame.Value, Vector3.forward, Mathf.Infinity, _droppableLayerMask);
            
            Droppable droppable = raycastHit ? raycastHit.transform.GetComponent<Droppable>() : null;
            if(droppable != null) {
                droppable.OnDrop(this);
            } else {
                transform.position = _dragStart;
            }
        }

        public void Reset()
        {
            transform.position = _dragStart;
        }

        void Update() {
            if(_dragging) {
                transform.position = getMouseWorldPositionOnTransformLevel() + _dragOffset;
            }
        }

        private Vector3 getMouseWorldPositionOnTransformLevel()
        {
            Vector3 pos = _mouseWorldPosInMinigame.Value;
            pos.z = transform.position.z;
            return pos;
        }
    }
}
