using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    //================== SINGLETON ==================
    public static MySceneManager _inst; // instância global (acesso rápido)

    public void Awake()
    {
        // verifica se já existe outra instância
        if (_inst == null)
        {
            _inst = this;
            DontDestroyOnLoad(gameObject); // persiste entre cenas (opcional)
        }
        else
        {
            Destroy(gameObject); // evita duplicatas
        }
    }

    //================== LOAD POR NOME ==================
    public void LoadScene(string _scnName)
    {
        // carrega a cena pelo nome
        SceneManager.LoadScene(_scnName);
    }

    //================== LOAD POR INDEX ==================
    public void LoadScene(int _scnIdx)
    {
        // verifica se o índice é válido
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
        // recarrega a cena atual
        int _idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(_idx);
    }

    //================== SAIR DO JOGO ==================
    public void QuitGame()
    {
        // sai do jogo (só funciona no build, no editor não faz nada)
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}
