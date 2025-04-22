using System;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AE._Project.Scripts.UI.CursorManagement
{
    /// <summary>
    ///     Represents data for <see cref="CursorInteraction" />
    /// </summary>
    [Serializable]
    public class CursorInteractionData : CursorAnimData
    {
        [FormerlySerializedAs("cursorSizeModifier")]
        public float _cursorSizeModifier;

        [FormerlySerializedAs("duration")] public float _duration;
    }

    public class CursorInteraction : CursorAnimationWithData<CursorInteractionData>
    {
        private Tween _anim;


        public CursorInteraction(Image cursorImage, CursorInteractionData data) : base(cursorImage, data)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            var enlargedCursorSize = _cursorImage.rectTransform.localScale * Data._cursorSizeModifier;
            _anim = DOTween
                .To(() => _cursorImage.transform.localScale, x => _cursorImage.transform.localScale = x,
                    enlargedCursorSize, Data._duration).SetLoops(-1, LoopType.Yoyo);
        }

        public override void OnExit()
        {
            base.OnExit();
            _anim?.Kill();
        }
    }
}