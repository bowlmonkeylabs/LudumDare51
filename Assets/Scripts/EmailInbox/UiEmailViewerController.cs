using System;
using BML.ScriptableObjectCore.Scripts.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace EmailInbox
{
    public class UiEmailViewerController : MonoBehaviour
    {
        #region Inspector

        [SerializeField, ReadOnly] private EmailItem _emailData;
        
        [SerializeField] private DynamicGameEvent _onOpenEmail;
        
        [Required, SerializeField] private TMP_Text _textFromAddress;
        [Required, SerializeField] private TMP_Text _textSubject;
        [Required, SerializeField] private TMP_Text _textBody;
        [Required, SerializeField] private Canvas _parentCanvas;

        #endregion

        #region Unity lifecycle

        private void Awake()
        {
            _emailData = null;
            RenderEmailData();
            _onOpenEmail.Subscribe(OnOpenEmailDynamic);
        }

        private void OnDestroy()
        {
            _onOpenEmail.Unsubscribe(OnOpenEmailDynamic);
        }

        #endregion
        
        private void OnOpenEmailDynamic(object prevValue, object currentValue)
        {
            var curr = currentValue as EmailItem;
            OnOpenEmail(curr);
        }

        private void OnOpenEmail(EmailItem emailItem)
        {
            _emailData = emailItem;
            RenderEmailData();
        }

        private void RenderEmailData()
        {
            var showThisObject = (_emailData != null);
            _parentCanvas.enabled = showThisObject;
            
            if (!showThisObject) return;

            _textFromAddress.text = _emailData.FromAddress;
            _textSubject.text = _emailData.Subject;
            _textBody.text = _emailData.Body;
        }
    }
}