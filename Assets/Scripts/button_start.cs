using UnityEngine;
using UnityEngine.UI;

public class button_start : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] GameStateSO gameState;

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
        gameState.StartGame();
    }
}