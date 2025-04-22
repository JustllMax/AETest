using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AE._Project.Scripts.UI.CursorManagement
{
    public enum CursorType
    {
        DefaultCursor = 0,
        InteractionCursor = 1
    }

    /// <summary>
    ///     Represents base data class for cursor animation's data
    /// </summary>
    public abstract class CursorAnimData
    {
        public Color CursorColor;
        public Sprite CursorIcon;
    }

    /// <summary>
    ///     Represents base class for cursor animations
    /// </summary>
    /// <remarks>Class should be abstract, but Unity Inspector cannot show abstract classes</remarks>
    [Serializable]
    public class CursorAnimation
    {
        [FormerlySerializedAs("cursorImage")] public Image _cursorImage;

        [FormerlySerializedAs("defaultCursorScale")]
        public Vector3 _defaultCursorScale;

        public CursorAnimation(Image cursorImage)
        {
            _cursorImage = cursorImage;
            _defaultCursorScale = cursorImage.rectTransform.localScale;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
    }

    [Serializable]
    public class CursorAnimationWithData<T> : CursorAnimation
        where T : CursorAnimData
    {
        public CursorAnimationWithData(Image cursorImage, T data) : base(cursorImage)
        {
            Data = data;
        }

        public T Data { get; }

        public override void OnEnter()
        {
            _cursorImage.sprite = Data.CursorIcon;
            _cursorImage.color = Data.CursorColor;
            _cursorImage.transform.localScale = _defaultCursorScale;
        }

        public override void OnExit()
        {
        }
    }
}