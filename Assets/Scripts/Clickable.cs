using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts {
    [RequireComponent(typeof(Collider2D))]
    public class Clickable : MonoBehaviour
    {
        [SerializeField] private bool _stop;
        [SerializeField] private UnityEvent _onClick;

        public delegate void OnClick();
        public event OnClick _OnClick;

        public bool ShouldStop { get { return _stop; } }

        public void EmitClick() {
            _onClick.Invoke();
            _OnClick?.Invoke();
        }

        public void SubscribeOnClick(OnClick callback)
        {
            _OnClick += callback;
        }
        
        public void UnSubscribeOnClick(OnClick callback)
        {
            _OnClick -= callback;
        }
    }
}
