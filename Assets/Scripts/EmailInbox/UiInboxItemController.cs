using System;
using BML.ScriptableObjectCore.Scripts.Events;
using IKVM.Reflection;
using Sirenix.OdinInspector;
using ThisOtherThing.UI.Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace EmailInbox
{
    public partial class UiInboxItemController : MonoBehaviour
    {
        #region Inspector

        public EmailItem EmailData
        {
            get => _emailData;
            set
            {
                _emailData = value;
                RenderEmailData();
            }
        }
        [SerializeField, ReadOnly] EmailItem _emailData;

        [SerializeField, ReadOnly] public bool IsSelected;

        [Required, SerializeField] private DynamicGameEvent _openEmail;
        [Required, SerializeField] private TMP_Text _textFromAddress;
        [Required, SerializeField] private TMP_Text _textSubject;
        [Required, SerializeField] private Button _button;
        [Required, SerializeField] private Rectangle _selectedOverrideRectangle; 

        #endregion

        #region Unity lifecycle

        #endregion
        
        #region Email data

        private void RenderEmailData()
        {
            var showThisObject = (_emailData != null);
            this.gameObject.SetActive(showThisObject);
            
            if (!showThisObject) return;

            _textFromAddress.text = _emailData.FromAddress;
            _textSubject.text = _emailData.Subject;

            var color = (IsSelected ? _button.colors.selectedColor : _button.colors.normalColor);
            // if (_button?.targetGraphic != null) _button.targetGraphic.color = color;
            _selectedOverrideRectangle.color = color;
            _selectedOverrideRectangle.gameObject.SetActive(IsSelected);
        }

        public void OpenEmail()
        {
            var payload = new EmailInstancePayload
            {
                InstanceId = this.GetInstanceID(),
                EmailData = _emailData,
            };
            _openEmail.Raise(payload);
        }
        
        public void UpdateColor() {
            var color = (IsSelected ? _button.colors.selectedColor : _button.colors.normalColor);
            _selectedOverrideRectangle.color = color;
            _selectedOverrideRectangle.gameObject.SetActive(IsSelected);
        }
        
        #endregion

    }
}