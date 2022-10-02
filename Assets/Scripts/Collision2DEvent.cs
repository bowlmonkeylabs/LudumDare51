using System;
using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts
{
    public class Collision2DEvent : MonoBehaviour
    {

            [SerializeField] private string _tag;
            [SerializeField] private UnityEvent<GameObject> _onCollision2DEnter;
            [SerializeField] private UnityEvent<GameObject> _onCollision2DExit;
            [SerializeField] private UnityEvent<GameObject> _onTrigger2DEnter;
            [SerializeField] private UnityEvent<GameObject> _onTrigger2DExit;

            protected void OnCollisionEnter2D(Collision2D other)
            {
                if (!other.gameObject.CompareTag(_tag)) return;
                _onCollision2DEnter.Invoke(other.gameObject);
            }

            protected void OnCollisionExit2D(Collision2D other)
            {
                if (!other.gameObject.CompareTag(_tag)) return;
                _onCollision2DExit.Invoke(other.gameObject);
            }
            
            protected void OnTriggerEnter2D(Collider2D other)
            {
                if (!other.gameObject.CompareTag(_tag)) return;
                _onTrigger2DEnter.Invoke(other.gameObject);
            }

            protected void OnTriggerExit2D(Collider2D other)
            {
                if (!other.gameObject.CompareTag(_tag)) return;
                _onTrigger2DExit.Invoke(other.gameObject);
            }
    }
    
}