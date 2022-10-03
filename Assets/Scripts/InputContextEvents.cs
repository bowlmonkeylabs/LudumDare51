using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BML.Scripts
{
    public class InputContextEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onStarted;
        [SerializeField] private UnityEvent _onPerformed;
        [SerializeField] private UnityEvent _onCancelled;
        
        public void ReceiveInput(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                _onStarted.Invoke();
            else if (ctx.performed)
                _onPerformed.Invoke();
            else if (ctx.canceled)
                _onCancelled.Invoke();
        }
    }
}