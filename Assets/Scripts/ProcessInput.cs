using UnityEngine.InputSystem;
using UnityEngine;

namespace BML.Scripts {
    public class ProcessInput : MonoBehaviour
    {
        [SerializeField] private LayerMask _interactableLayerMask;

        private bool _dragging;

        public void OnClick(InputAction.CallbackContext context) {
            if(context.performed) {
                Ray clickRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit2D[] raycastHits = Physics2D.RaycastAll(clickRay.origin, clickRay.direction, Mathf.Infinity, _interactableLayerMask);
                    
                foreach(RaycastHit2D raycastHit in raycastHits) {
                    Clickable clickable = raycastHit.transform.GetComponent<Clickable>();
                    if(clickable != null) {
                        clickable.EmitClick();
                        if(clickable.ShouldStop) {
                            return;
                        }
                    }
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

