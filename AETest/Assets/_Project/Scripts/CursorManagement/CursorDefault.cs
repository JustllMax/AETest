using System;
using UnityEngine;
using UnityEngine.UI;

namespace AE.CursorManagement
{

    
    /// <summary>
    /// Class for marking default data
    /// </summary>
    [Serializable]
    public class DefaultCursorData : CursorAnimData
    {
        
    }
    
    /// <summary>
    /// Default cursor, without any special animation
    /// </summary>
    [Serializable]
    public class CursorDefault : CursorAnimationWithData<DefaultCursorData>
    {
        public CursorDefault(Image cursorImage, DefaultCursorData data) : base(cursorImage, data) { }
        
    }
}
