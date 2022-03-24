
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class SimpleJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    protected RectTransform image;
    protected Vector2 startPosition, direction;
    protected bool pressed;
    public Vector2 Direction => direction;
    public bool Pressed => pressed;
    public virtual void OnDrag(PointerEventData eventData)
    {       
        direction = (eventData.position - startPosition).normalized;
       
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        startPosition = eventData.position;
        image.position = eventData.position;
        pressed = true;
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        direction = Vector2.zero;
        pressed = false;
    }
}