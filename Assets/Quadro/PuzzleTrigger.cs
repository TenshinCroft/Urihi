using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("Referências")]
    public GameObject puzzleUI;

    [HideInInspector] public bool _bool = false;
    private bool isOpen = false;

    private void Update()
    {
        // Não permite abrir ou fechar puzzle se o jogo estiver pausado
        if (SettingsMenu.isPaused) return;

        if (_bool && !isOpen) OpenPuzzle();

        if (_bool && isOpen && Input.GetKeyDown(KeyCode.F))
        {
            ClosePuzzle();
            _bool = false;
        }
    }

    public void OpenPuzzle()
    {
        if (puzzleUI != null) puzzleUI.SetActive(true);
        isOpen = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Puzzle aberto!");
    }

    public void ClosePuzzle()
    {
        if (puzzleUI != null) puzzleUI.SetActive(false);
        isOpen = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Puzzle fechado!");
    }

    public void FecharPuzzle()
    {
        if (isOpen) ClosePuzzle();
        _bool = false;
    }
}
