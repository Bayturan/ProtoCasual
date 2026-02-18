using UnityEngine;

namespace ProtoCasual.Core.Interfaces
{
    public enum InputType { Tap, Swipe, Drag, Hold, Steering }
    
    public interface IInputService
    {
        bool IsInputEnabled { get; set; }
        Vector2 GetInputPosition();
        Vector2 GetDragDelta();
        float GetHorizontalAxis();
        float GetVerticalAxis();
        
        event System.Action<Vector2> OnTap;
        event System.Action<Vector2, Vector2> OnSwipe;
        event System.Action<Vector2> OnDragStart;
        event System.Action<Vector2> OnDrag;
        event System.Action OnDragEnd;
        event System.Action<Vector2> OnHoldStart;
        event System.Action<Vector2> OnHold;
        event System.Action OnHoldEnd;
    }
}
