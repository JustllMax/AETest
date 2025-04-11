using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AE
{
    public enum LightMode
    {
        Static,
        Pulsating
    }
    
    [Serializable]
    public class PulseData
    {
        [SerializeField] public Color pulseColor = Color.yellow;
        [SerializeField] public float pulseTemperature;
        [SerializeField, Min(0.1f)] public Vector2 pulseFrequencyRange = new Vector2(5, 10);
        [SerializeField, Min(0.2f)] public float pulseDuration = 1f;
        [SerializeField, Range(1.0f, 100f)] public float pulsatingIntensityModifier = 1.0f;
        [SerializeField, Range(1.0f, 100f)] public float pulsatingRangeModifier = 1.0f;
    }

    [RequireComponent(typeof(Light))]
    public class LightComponent : MonoBehaviour
    {
  
        
        [Header("Light Settings")]
        [SerializeField] LightMode lightMode = LightMode.Static;

        [SerializeField] private PulseData pulseData = new PulseData();

        [Header("Debug")] 
        [SerializeField] private Light _light;
        [SerializeField] private float baseIntensity;
        [SerializeField] private float baseRange;
        [SerializeField] private float baseTemperature;
        [SerializeField] private Color baseColor;
        [SerializeField] private Color currentColor;

        public PulseData PulseData => pulseData;
        
        private CancellationTokenSource _cts = new CancellationTokenSource();
        
        public Light Light => _light;
        
        Tween colorChangeTween;
        private float PulseRange => pulseData.pulsatingRangeModifier * baseRange;
        private float PulseIntensity => pulseData.pulsatingIntensityModifier * baseIntensity;

        private void Awake()
        {
            _light = GetComponent<Light>();
            baseRange = _light.range;
            baseIntensity = _light.intensity;
            baseColor = _light.color;
            baseTemperature = _light.colorTemperature;

            currentColor = baseColor;
        }

        private void Start()
        {
            ChangeLightMode(lightMode, true);
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
            if(lightMode == newMode && !forceChange) return; 
            lightMode = newMode;
            switch (lightMode)
            {
                case  LightMode.Static:
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
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }
         
        private void PulseLight()
        {
            StopPulse();
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
                    float delay = Random.Range(pulseData.pulseFrequencyRange.x, pulseData.pulseFrequencyRange.y);
                    await UniTask.WaitForSeconds(delay, cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("LoopPulse cancelled");
            }
        }

        private async UniTask IncreasePulse(CancellationToken token)
        {
            
            float halfDuration = pulseData.pulseDuration / 2f;
            Sequence sequence = DOTween.Sequence();
            
            // Intensity
            sequence.Append(_light.DOIntensity(PulseIntensity, halfDuration));
            
            // Temperature
            if (_light.useColorTemperature)
                sequence.Join(DOTween.To(() => _light.colorTemperature, x => _light.colorTemperature = x, pulseData.pulseTemperature, halfDuration));
            // Color
            else sequence.Join(_light.DOColor(pulseData.pulseColor, halfDuration));
            
            // Range
            sequence.Join(DOTween.To(() => _light.range, x => _light.range = x, PulseRange, halfDuration));

            try
            {
                await sequence.AsyncWaitForCompletion()
                    .AsUniTask()
                    .AttachExternalCancellation(token);
            }
            catch (OperationCanceledException)
            {
                sequence.Kill();
            }
        }

        private async UniTask DecreasePulse(CancellationToken token)
        {
            float halfDuration = pulseData.pulseDuration / 2f;
            
            Sequence sequence = DOTween.Sequence();
            // Intensity
            sequence.Append(_light.DOIntensity(baseIntensity, halfDuration));
            // Temperature
            if (_light.useColorTemperature)
                sequence.Join(DOTween.To(() => _light.colorTemperature, x => _light.colorTemperature = x, baseTemperature, halfDuration));
            // Color
            else sequence.Join(_light.DOColor(currentColor, halfDuration));

            // Range
            sequence.Join(DOTween.To(() => _light.range, x => _light.range = x, baseRange, halfDuration));
            
            try
            {
                await sequence
                    .AsyncWaitForCompletion()
                    .AsUniTask()
                    .AttachExternalCancellation(token);
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

        public void ResetColorToDefault() => SetCurrentColor(baseColor);
        public void SetCurrentColor(Color color, float colorDurationChange = 1f)
        {
            colorChangeTween?.Kill();
            
            currentColor = color;
            
            colorChangeTween = _light.DOColor(currentColor, colorDurationChange);
        }
        private void OnDestroy()
        {
            StopPulse();
            colorChangeTween?.Kill();
            _cts.Dispose();
        }
    }
    
}
