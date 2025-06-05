using UnityEngine;

public class porta : MonoBehaviour
{
    private bool _aberta = false;

    public void AbrirPorta()
    {
        if (!_aberta)
        {
            transform.Rotate(0f, 90f, 0f); // gira 90 graus no eixo Y
            _aberta = true;
        }
    }
}
