using UnityEngine;
using UnityEngine.UI;

public class button_restart : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] GameStateSO gameState;

    public Button myButton; // Assign this in the Inspector

    void Start()
    {
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        gameState.RestartGame();
    }
}