using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string proximafase;
    public GameObject[] itensmenu;
    public void StartGame()
    {
        SceneManager.LoadScene(proximafase);
    }
    public void configuracoes()
    {


    }
}
