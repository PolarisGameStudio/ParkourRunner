using UnityEngine;
using UnityEngine.EventSystems;
using Adverty;

[RequireComponent(typeof(MenuUnit))]
public class MenuUnitClickHandler : MonoBehaviour, IPointerClickHandler
{
    private MenuUnit menuUnit;
    private RectTransform rectTransform;
    private const float CENTER_POINT = 0.5f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        menuUnit = GetComponent<MenuUnit>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPos;
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPos))
        {
            return;
        }
        Vector2 normalizedPosition = GeneratePositionPoint(localPos);
        menuUnit.Interact(normalizedPosition);
    }

    private Vector2 GeneratePositionPoint(Vector2 localPoint)
    {
        Vector2 size = rectTransform.sizeDelta;
        float x = ((localPoint.x + size.x * CENTER_POINT) / size.x) - (CENTER_POINT - rectTransform.pivot.x);
        float y = 1.0f - (((localPoint.y + size.y * CENTER_POINT) / size.y) - (CENTER_POINT - rectTransform.pivot.y));
        return new Vector2(x, y);
    }
}
