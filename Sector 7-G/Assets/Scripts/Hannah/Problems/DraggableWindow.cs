using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler
{
    [Header("Window To Move")]
    [SerializeField] private Transform window;

    private Vector3 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (window == null)
            return;

        offset = window.position -
                 (Vector3)eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (window == null)
            return;

        window.position =
            (Vector3)eventData.position + offset;
    }
}