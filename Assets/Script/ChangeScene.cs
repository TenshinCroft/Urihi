using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public void GoToScene()
    {
        SceneManager.LoadScene("Ambiente");
    }
}
