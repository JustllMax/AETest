using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace AE._Project.Scripts.Components
{
    public enum LightMode
    {
        Static,
        Pulsating
    }

    [Serializable]
    public sealed class PulseData
    {
        [FormerlySerializedAs("pulseColor")] [SerializeField]
        public Color _pulseColor = Color.yellow;

        [FormerlySerializedAs("pulseTemperature")] [SerializeField]
        public float _pulseTemperature;

        [FormerlySerializedAs("pulseFrequencyRange")] [SerializeField] [Min(0.1f)]
        public Vector2 _pulseFrequencyRange = new(5, 10);

        [FormerlySerializedAs("pulseDuration")] [SerializeField] [Min(0.2f)]
        public float _pulseDuration = 1f;

        [FormerlySerializedAs("pulsatingIntensityModifier")] [SerializeField] [Range(1.0f, 100f)]
        public float _pulsatingIntensityModifier = 1.0f;

        [FormerlySerializedAs("pulsatingRangeModifier")] [SerializeField] [Range(1.0f, 100f)]
        public float _pulsatingRangeModifier = 1.0f;
    }

    /// <summary>
    ///     Handles light settings
    /// </summary>
    [RequireComponent(typeof(Light))]
    public sealed class LightComponent : MonoBehaviour
    {
        [FormerlySerializedAs("lightMode")] [Header("Light Settings")] [SerializeField]
        private LightMode _lightMode = LightMode.Static;

        [FormerlySerializedAs("pulseData")] [SerializeField]
        private PulseData _pulseData = new();

        // [Header("Debug")] 
        [SerializeField] private Light _light;

        [FormerlySerializedAs("baseIntensity")] [SerializeField]
        private float _baseIntensity;

        [FormerlySerializedAs("baseRange")] [SerializeField]
        private float _baseRange;

        [FormerlySerializedAs("baseTemperature")] [SerializeField]
        private float _baseTemperature;

        [FormerlySerializedAs("baseColor")] [SerializeField]
        private Color _baseColor;

        [FormerlySerializedAs("currentColor")] [SerializeField]
        private Color _currentColor;

        private Tween _colorChangeTween;

        private CancellationTokenSource _cts = new();

        public PulseData PulseData => _pulseData;

        public Light Light => _light;
        private float PulseRange => _pulseData._pulsatingRangeModifier * _baseRange;
        private float PulseIntensity => _pulseData._pulsatingIntensityModifier * _baseIntensity;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _baseRange = _light.range;
            _baseIntensity = _light.intensity;
            _baseColor = _light.color;
            _baseTemperature = _light.colorTemperature;

            _currentColor = _baseColor;
        }

        private void Start() => ChangeLightMode(_lightMode, true);

        private void OnDestroy()
        {
            DOTween.Kill(this);
            _cts?.Cancel();
            _cts?.Dispose();
        }


        /* NOTE: Used during tests
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.G))
            {
                ChangeLightMode(LightMode.Static, true);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                ChangeLightMode(LightMode.Pulsating, true);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Destroy(gameObject);
            }
        }
        */
        public void ChangeLightMode(LightMode newMode, bool forceChange = false)
        {
            if (_lightMode == newMode && !forceChange)
            {
                return;
            }

            _lightMode = newMode;
            switch (_lightMode)
            {
                case LightMode.Static:
                    StaticLight();
                    return;
                case LightMode.Pulsating:
                    PulseLight();
                    return;
            }
        }

        private void StaticLight()
        {
            StopPulse();
        }

        private void StopPulse()
        {
            if (_cts is { IsCancellationRequested: false })
            {
                _cts.Cancel();
            }
        }

        private void PulseLight()
        {
            LoopPulse(_cts.Token).Forget();
        }

        private async UniTaskVoid LoopPulse(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    await IncreasePulse(token);
                    await DecreasePulse(token);
                    var delay = Random.Range(_pulseData._pulseFrequencyRange.x, _pulseData._pulseFrequencyRange.y);
                    await UniTask.WaitForSeconds(delay, cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("LoopPulse cancelled");
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }
        }

        private async UniTask IncreasePulse(CancellationToken token)
        {
            var halfDuration = _pulseData._pulseDuration / 2f;
            var sequence = DOTween.Sequence();

            // Intensity
            sequence.Append(_light.DOIntensity(PulseIntensity, halfDuration));

            // Temperature
            if (_light.useColorTemperature)
            {
                sequence.Join(DOTween.To(() => _light.colorTemperature, x => _light.colorTemperature = x,
                    _pulseData._pulseTemperature, halfDuration));
            }
            // Color
            else
            {
                sequence.Join(_light.DOColor(_pulseData._pulseColor, halfDuration));
            }

            // Range
            sequence.Join(DOTween.To(() => _light.range, x => _light.range = x, PulseRange, halfDuration));

            try
            {
                await sequence.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                sequence.Kill();
            }
        }

        private async UniTask DecreasePulse(CancellationToken token)
        {
            var halfDuration = _pulseData._pulseDuration / 2f;

            var sequence = DOTween.Sequence();
            // Intensity
            sequence.Append(_light.DOIntensity(_baseIntensity, halfDuration));
            // Temperature
            if (_light.useColorTemperature)
            {
                sequence.Join(DOTween.To(() => _light.colorTemperature, x => _light.colorTemperature = x,
                    _baseTemperature, halfDuration));
            }
            // Color
            else
            {
                sequence.Join(_light.DOColor(_currentColor, halfDuration));
            }

            // Range
            sequence.Join(DOTween.To(() => _light.range, x => _light.range = x, _baseRange, halfDuration));

            try
            {
                await sequence.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                sequence.Kill();
            }
        }

        public void SetUseColorTemperature(bool value)
        {
            _light.useColorTemperature = value;
        }

        public void ResetColorToDefault()
        {
            SetCurrentColor(_baseColor);
        }

        public void SetCurrentColor(Color color, float colorDurationChange = 1f)
        {
            _colorChangeTween?.Kill();

            _currentColor = color;

            _colorChangeTween = _light.DOColor(_currentColor, colorDurationChange);
        }
    }
}