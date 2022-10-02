using TMPro;
using UnityEngine;
using BML.ScriptableObjectCore.Scripts.Variables;

namespace BML.Scripts {
    [RequireComponent(typeof(TMP_Text))]
    public class GameTimeRenderer : MonoBehaviour
    {
        private const float EIGHT_HOURS_IN_SECONDS = 28800;

        [SerializeField] private TimerVariable _gameTimer;
        [SerializeField] private TMP_Text _textComp;

        private float _timerStep;

        void OnEnable() 
        {
            _timerStep = EIGHT_HOURS_IN_SECONDS / _gameTimer.Duration;

            _gameTimer.Subscribe(UpdateText);
        }

        void OnDisable() 
        {
            _gameTimer.Unsubscribe(UpdateText);
        }

        private void UpdateText() 
        {
            var time = (int)(_gameTimer.ElapsedTime * 48);
            var hours = time / 60 / 60;
            var minutes = time / 60;
            
            _textComp.text = $"{$"{hours}".PadLeft(2, '0')}:{$"{minutes}".PadLeft(2, '0')}";
        }
    }
}
