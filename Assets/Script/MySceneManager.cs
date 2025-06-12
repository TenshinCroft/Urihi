using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    //================== SINGLETON ==================
    public static MySceneManager _inst; // inst�ncia global (acesso r�pido)

    public void Awake()
    {

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
        // verifica se o �ndice � v�lido
        if (_scnIdx >= 0 && _scnIdx < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(_scnIdx);
        }
        else
        {
            Debug.LogWarning("�ndice de cena inv�lido: " + _scnIdx);
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
        // sai do jogo (s� funciona no build, no editor n�o faz nada)
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}
