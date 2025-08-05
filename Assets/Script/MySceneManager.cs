using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    //================== SINGLETON ==================
    public static MySceneManager _inst; // instância global

    void Awake()
    {
        if (_inst == null)
        {
            _inst = this;
            DontDestroyOnLoad(gameObject); // mantém entre cenas
        }
        else
        {
            Destroy(gameObject); // evita duplicatas
        }
    }

    //================== LOAD POR NOME ==================
    public void LoadScene(string _scnName)
    {
        SceneManager.LoadScene(_scnName);
    }

    //================== LOAD POR INDEX ==================
    public void LoadScene(int _scnIdx)
    {
        if (_scnIdx >= 0 && _scnIdx < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(_scnIdx);
        }
        else
        {
            Debug.LogWarning("Índice de cena inválido: " + _scnIdx);
        }
    }

    //================== RECARREGAR A ATUAL ==================
    public void ReloadScene()
    {
        int _idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(_idx);
    }

    //================== SAIR DO JOGO ==================
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}
