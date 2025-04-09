using System;
using UnityEngine;
using UnityEngine.UI;

namespace AE.CursorManagement
{
    public enum CursorType
    {
        DefaultCursor = 0,
        InteractionCursor = 1,
    }
    
    /// <summary>
    /// Represents base data class for cursor animation's data
    /// </summary>
    public abstract class CursorAnimData
    {
        public Sprite cursorIcon;
        public Color cursorColor;
    }
    
    /// <summary>
    /// Represents base class for cursor animations
    /// </summary>
    /// <remarks>Class should be abstract, but Unity Inspector cannot show abstract classes</remarks>
    [Serializable]
    public  class CursorAnimation
    {
        
        public Image cursorImage;
        public Vector3 defaultCursorScale;

        public CursorAnimation(Image cursorImage)
        {
            this.cursorImage = cursorImage;
            defaultCursorScale = cursorImage.rectTransform.localScale;
        }
        
        public virtual void OnEnter() { }
        
        public virtual void OnExit(){}
    }
    
    [Serializable]
    public class CursorAnimationWithData<T> : CursorAnimation
        where T : CursorAnimData
    {
        
        private T data;
        public T Data => data;
        public CursorAnimationWithData(Image cursorImage, T data) : base(cursorImage)
        {
            this.data = data;
        }
        
        public override void OnEnter()
        {
            cursorImage.sprite = data.cursorIcon;
            cursorImage.color = data.cursorColor;
            cursorImage.transform.localScale = defaultCursorScale;
        }
        
        public override void OnExit(){}
    }
}
