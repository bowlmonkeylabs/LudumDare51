using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using UnityEngine.InputSystem;
using UnityEngine;

namespace BML.Scripts {
    public class ProcessInput : MonoBehaviour
    {
        [SerializeField] private LayerMask _interactableLayerMask;
        [SerializeField] private Vector3Variable _mouseWorldPosInMinigame;

        private bool _dragging;

        private void Update()
        {
            _mouseWorldPosInMinigame.Value = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }

        public void OnClick(InputAction.CallbackContext context) {
            if(context.performed) {
                Ray clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit2D raycastHit = Physics2D.Raycast(clickRay.origin, clickRay.direction, Mathf.Infinity, _interactableLayerMask);
                Clickable clickable = raycastHit.transform.GetComponent<Clickable>();
                if(clickable != null) {
                    clickable.EmitClick();
                }
            }
        }

        public void OnDrag(InputAction.CallbackContext context) {
            if(context.performed) {
                _dragging = true;
                Ray clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit2D raycastHit = Physics2D.Raycast(clickRay.origin, clickRay.direction, Mathf.Infinity, _interactableLayerMask);
                if(raycastHit) {
                    Draggable draggable = raycastHit.transform.GetComponent<Draggable>();
                    if(draggable != null) {
                        draggable.StartDrag();
                    }
                }
            }
            if(_dragging && context.canceled) {
                _dragging = false;
                Ray clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit2D raycastHit = Physics2D.Raycast(clickRay.origin, clickRay.direction, Mathf.Infinity, _interactableLayerMask);
                if(raycastHit) {
                    Draggable draggable = raycastHit.transform.GetComponent<Draggable>();
                    if(draggable != null) {
                        draggable.EndDrag();
                    }
                }
            }
        }
    }
}

