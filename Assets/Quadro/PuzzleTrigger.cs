using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    [Header("Referências")]
    public GameObject _puzzleUI; // painel do puzzle (Canvas)

    public bool _isPlayerNear = false;
    public bool _bool = false;

    void Update()
    {
        // verifica se o player está perto e apertou a tecla
        //if (_isPlayerNear && Input.GetKeyDown(KeyCode.F))
        if (Input.GetKeyDown(KeyCode.F))
        {
            //_bool = !_bool;
            _bool = false;
            //if (_bool)
            //{
            //    AbrirPuzzle();
            //}
            //else
            //{
            //    FecharPuzzle();
            //}
        }
        if (_bool)
        {
            AbrirPuzzle();
        }
        else
        {
            FecharPuzzle();
        }
    }

    void AbrirPuzzle()
    {
        _puzzleUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // libera cursor
        Cursor.visible = true;
        Time.timeScale = 0f; // pausa o jogo
    }

    public void FecharPuzzle()
    {
        _puzzleUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}
