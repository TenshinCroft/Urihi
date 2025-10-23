using UnityEngine;

public class quit : MonoBehaviour
{
    public void SairDoJogo()
    {
        // Este comando faz com que o aplicativo feche.
        Application.Quit();

        // IMPORTANTE: Este comando SÓ FUNCIONA quando o jogo está "buildado" (compilado) para ser executado
        // como um aplicativo (por exemplo, um .exe, .apk, etc.).
        // Ele NÃO FUNCIONA quando você clica em Play dentro do editor da Unity.

        // Para saber se a função foi chamada no editor (e não fechar o Unity):
#if UNITY_EDITOR
        Debug.Log("O jogo seria fechado se estivesse buildado!");
        // Se você realmente quiser parar o jogo no editor (apenas para teste, não fechará o editor),
        // você pode usar: UnityEditor.EditorApplication.isPlaying = false;
        // Mas isso exige adicionar `using UnityEditor;` e só funciona no editor.
#endif
    }
}
