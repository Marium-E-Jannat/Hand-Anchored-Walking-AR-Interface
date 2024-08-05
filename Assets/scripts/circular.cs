using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CircularButtonLayout : MonoBehaviour
{
    public Button startButton;  
    public Button[] buttons;   
    private float radius = 100f; 
    private float buttonRadius = 50f;

    public void SetDistRadius(float value) {
        radius = value;
        HandleLayoutChange();
    }

    public void SetButtonRadius(float value)
    {
        buttonRadius = value;
        HandleLayoutChange();
    }

    void HandleLayoutChange()
    {
        
        startButton.transform.localPosition = new Vector3(0,0,-1);

        RectTransform startButtonRect = startButton.GetComponent<RectTransform>();
        startButtonRect.sizeDelta = new Vector2(buttonRadius * 2, buttonRadius * 2);
        float angleStep = 360f / buttons.Length;
        float angle = 0f;

       
        for (int i = 0; i < buttons.Length; i++)
        {
            
            float x = (radius+buttonRadius) * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = (radius+buttonRadius) * Mathf.Sin(angle * Mathf.Deg2Rad);

            
            buttons[i].transform.localPosition = new Vector3(x, y, -1);
            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(buttonRadius * 2, buttonRadius * 2);

           
            angle += angleStep;
        }
    }
}
