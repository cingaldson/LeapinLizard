using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public GameManagerLiz gameManager;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        gameManager.StartGame();
        gameObject.SetActive(false); // Disable the start button after it is pressed
    }
}