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

    [RequireComponent(typeof(Light))]
    public class LightComponent : MonoBehaviour
    {
       
        
        
        [Header("Light Settings")]
        [SerializeField] LightMode lightMode = LightMode.Static;
        [Header("Pulsating Settings")]
        [SerializeField] private Color pulseColor = Color.yellow;
        [SerializeField] private float pulseTemperature;
        [SerializeField, Min(0.1f)] Vector2 pulseFrequencyRange = new Vector2(5, 10);
        [SerializeField, Min(0.2f)] float pulseDuration = 1f;
        [SerializeField, Range(1.0f, 100f)] float pulsatingIntensityModifier = 1.0f;
        [SerializeField, Range(1.0f, 100f)] float pulsatingRangeModifier = 1.0f;

        [Header("Debug")] 
        [SerializeField] private Light _light;
        [SerializeField] private float baseIntensity;
        [SerializeField] private float baseRange;
        [SerializeField] private float baseTemperature;
        [SerializeField] private Color baseColor;
        
        private CancellationTokenSource _cts;
        
        public Light Light => _light;


        private float PulseRange => pulsatingRangeModifier * baseRange;
        private float PulseIntensity => pulsatingIntensityModifier * baseIntensity;

        private void Awake()
        {
            _light = GetComponent<Light>();
            baseRange = _light.range;
            baseIntensity = _light.intensity;
            baseColor = _light.color;
            baseTemperature = _light.colorTemperature;
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
            //DOTween.Kill(this);
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
         
        private void PulseLight()
        {
            StopPulse();
            _cts = new CancellationTokenSource();
            LoopPulse(_cts.Token);
        }

        private async UniTask LoopPulse(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    await IncreasePulse(token);
                    await DecreasePulse(token);
                    float delay = Random.Range(pulseFrequencyRange.x, pulseFrequencyRange.y);
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
            
            float halfDuration = pulseDuration / 2f;
            Sequence sequence = DOTween.Sequence();
            
            // Intensity
            sequence.Append(_light.DOIntensity(PulseIntensity, halfDuration));
            // Color
            sequence.Join(_light.DOColor(pulseColor, halfDuration));
            // Temperature
            if (_light.useColorTemperature)
                sequence.Join(DOTween.To(() => _light.colorTemperature, x => _light.colorTemperature = x, pulseTemperature, halfDuration));
            // Range
            sequence.Join(DOTween.To(() => _light.range, x => _light.range = x, PulseRange, halfDuration));

            try
            {
                await sequence.AsyncWaitForCompletion();
            }
            catch (OperationCanceledException)
            {
                sequence.Kill();
            }
        }

        private async UniTask DecreasePulse(CancellationToken token)
        {
            float halfDuration = pulseDuration / 2f;
            
            Sequence sequence = DOTween.Sequence();
            // Intensity
            sequence.Append(_light.DOIntensity(baseIntensity, halfDuration));
            // Color
            sequence.Join(_light.DOColor(baseColor, halfDuration));
            // Temperature
            if (_light.useColorTemperature)
                sequence.Join(DOTween.To(() => _light.colorTemperature, x => _light.colorTemperature = x, baseTemperature, halfDuration));
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

        private void OnDestroy()
        {
            StopPulse();
        }
    }
    
}
