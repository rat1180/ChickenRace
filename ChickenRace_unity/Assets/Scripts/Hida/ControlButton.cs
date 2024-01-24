using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlButton : MonoBehaviour
{
    [System.Serializable]
    public class ButtonListClass
    {
        public List<Button> buttons;
    }

    MenuManager menuManager;
    public Color outLineColor;
    public float outLineWidth;
    public List<ButtonListClass> buttons = new List<ButtonListClass>();
    int verticalIndex;
    int horizontalIndex;
    // Start is called before the first frame update
    void Start()
    {
        menuManager = GetComponent<MenuManager>();
        verticalIndex = horizontalIndex = 0;
        ButtonMove(Vector2.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonMove(Vector2 vec)
    {
        if (buttons == null || buttons.Count == 0) return;

        var list = buttons[verticalIndex];
        var line = list.buttons[horizontalIndex].GetComponent<Outline>();
        if(line != null)
        {
            line.enabled = false;
        }
        if (vec.y > 0)
        {
            verticalIndex++;
            if (buttons.Count <= verticalIndex) verticalIndex = 0;
        }
        else if(vec.y < 0)
        {
            verticalIndex--;
            if (0 > verticalIndex) verticalIndex = buttons.Count-1;
        }

        if(vec.x > 0)
        {
            horizontalIndex++;
            if (horizontalIndex >= list.buttons.Count) horizontalIndex = 0;
        }else if (vec.x < 0)
        {
            horizontalIndex--;
            if (horizontalIndex < 0) horizontalIndex = list.buttons.Count-1;
        }

        list = buttons[verticalIndex];
        line = list.buttons[horizontalIndex].gameObject.GetComponent<Outline>();
        if(line == null) line = list.buttons[horizontalIndex].gameObject.AddComponent<Outline>();
        line.effectColor = outLineColor;
        line.effectDistance = new Vector2(outLineWidth,-outLineWidth);
        line.enabled = true;
    }

    public void ButtonClick()
    {
        if (buttons == null || buttons.Count == 0) return;

        var list = buttons[verticalIndex];
        list.buttons[horizontalIndex].onClick.Invoke();
    }
}
