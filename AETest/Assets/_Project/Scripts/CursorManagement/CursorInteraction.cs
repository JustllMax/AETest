using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AE.CursorManagement
{
    /// <summary>
    /// Represents data for <see cref="CursorInteraction"/>
    /// </summary>
    [Serializable]
    public class CursorInteractionData : CursorAnimData
    {
        public float cursorSizeModifier ;
        public float duration;
    }
    public class CursorInteraction : CursorAnimationWithData<CursorInteractionData>
    {
        private Tween Anim;


        public CursorInteraction(Image cursorImage, CursorInteractionData data) : base(cursorImage, data)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Vector3 enlargedCursorSize = cursorImage.rectTransform.localScale * Data.cursorSizeModifier; 
            Anim = DOTween.To(() =>  cursorImage.transform.localScale, x => cursorImage.transform.localScale = x, enlargedCursorSize, Data.duration).SetLoops(-1, LoopType.Yoyo);
        }

        public override  void OnExit()
        {
            base.OnExit();
            Anim?.Kill();
        }
    }
}
