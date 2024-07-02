using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChangeButtonColorOnSelect : MonoBehaviour, ISelectHandler
{
    public Button button;

    // This method is called when the button is selected
    public void OnSelect(BaseEventData eventData)
    {
        if (button != null)
        {
            ColorBlock cb = button.colors;
            cb.selectedColor = Color.green; // Change this to the color you want
            button.colors = cb;
        }
    }

    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }
}
