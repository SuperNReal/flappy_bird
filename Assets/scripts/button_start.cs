using UnityEngine;
using UnityEngine.UI;

public class button_start : MonoBehaviour
{
    public Button myButton;

    void Start()
    {
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        GameManager.Instance.Begin();
    }
}