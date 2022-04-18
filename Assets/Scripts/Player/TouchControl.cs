using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControl : MonoBehaviour
{
    private static Vector3 touchPos = Vector3.zero;

    public static TouchState GetTouchState()
    {
        if (Application.isEditor)
        {
            if (EventSystem.current.IsPointerOverGameObject()) { return TouchState.None; }
            if (Input.GetMouseButtonDown(0)) { return TouchState.Start; }
            if (Input.GetMouseButton(0)) { return TouchState.Moved; }
            if (Input.GetMouseButtonUp(0)) { return TouchState.Ended; }
        }
        else
        {
            if (Input.touchCount > 0)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) { return TouchState.None; }
                return (TouchState)((int)Input.GetTouch(0).phase);
            }
        }
        return TouchState.None;
    }

    public static Vector3 GetTouchPosition()
    {
        if (Application.isEditor)
        {
            return Input.mousePosition;
        }
        else
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                touchPos.x = touch.position.x;
                touchPos.y = touch.position.y;
                return touchPos;
            }
        }
        return Vector3.zero;
    }
}

public enum TouchState
{
    Start = 0,
    Moved = 1,
    Stay = 2,
    Ended = 3,
    None = 9
}