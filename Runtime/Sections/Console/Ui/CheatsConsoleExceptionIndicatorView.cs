using UnityEngine;
using UnityEngine.UI;

namespace SMCheats
{
    [DisallowMultipleComponent]
    public sealed class CheatsConsoleExceptionIndicatorView : MonoBehaviour
    {
        [SerializeField] private Graphic _targetGraphic;
        [SerializeField] private float _flashDuration = 3f;
        [SerializeField] private float _blinkFrequency = 7f;
        [SerializeField] private bool _hideWhenIdle = true;

        private float _remainingFlashTime;
        private Graphic TargetGraphic => _targetGraphic != null ? _targetGraphic : GetComponent<Graphic>();

        private void Awake()
        {
            ResetState();
        }

        public void Flash()
        {
            _remainingFlashTime = Mathf.Max(_remainingFlashTime, Mathf.Max(0.1f, _flashDuration));
            gameObject?.SetActive(true);
        }

        private void Update()
        {
            if (_remainingFlashTime <= 0f)
            {
                return;
            }

            Tick(Time.unscaledDeltaTime);
        }

        private void Tick(float deltaTime)
        {
            if (_remainingFlashTime <= 0f)
            {
                return;
            }

            _remainingFlashTime = Mathf.Max(0f, _remainingFlashTime - Mathf.Max(0f, deltaTime));
            var elapsedTime = Mathf.Max(0f, _flashDuration - _remainingFlashTime);
            var blink = Mathf.PingPong(elapsedTime * Mathf.Max(0.1f, _blinkFrequency), 1f);
            var uiSettings = CheatsSettingsProxy.UiSettings;
            ApplyColor(Color.Lerp(
                uiSettings.ConsoleExceptionIndicatorIdleColor,
                uiSettings.ConsoleExceptionIndicatorFlashColor,
                blink));

            if (_remainingFlashTime <= 0f)
            {
                ResetState();
            }
        }

        private void ResetState()
        {
            _remainingFlashTime = 0f;
            ApplyColor(CheatsSettingsProxy.UiSettings.ConsoleExceptionIndicatorIdleColor);
            if (_hideWhenIdle)
            {
                gameObject.SetActive(false);
            }
        }

        private void ApplyColor(Color color)
        {
            TargetGraphic.color = color;
        }
    }
}
