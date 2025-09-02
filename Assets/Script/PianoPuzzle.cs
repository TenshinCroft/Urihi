using UnityEngine;
using UnityEngine.InputSystem;

public class PianoPuzzle : MonoBehaviour
{
    [Header("Ordem correta dos cubos (arraste aqui no Inspector)")]
    public GameObject[] ordemCorreta;

    [Header("Item que será liberado ao completar o puzzle")]
    public GameObject itemLiberado;

    private int indiceAtual = 0;
    private Rigidbody rbItem;

    void Start()
    {
        if (itemLiberado != null)
        {
            rbItem = itemLiberado.GetComponent<Rigidbody>();

            if (rbItem != null)
            {
                // Congela o item no início
                rbItem.isKinematic = true;
                rbItem.useGravity = false;
            }
        }
    }

    void Update()
    {
        Vector2 posicaoClique = Vector2.zero;
        bool clicou = false;

        // Clique do mouse
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            posicaoClique = Mouse.current.position.ReadValue();
            clicou = true;
        }

        // Toque na tela (mobile)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            posicaoClique = Touchscreen.current.primaryTouch.position.ReadValue();
            clicou = true;
        }

        // Se houve clique/toque
        if (clicou)
        {
            Ray ray = Camera.main.ScreenPointToRay(posicaoClique);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == ordemCorreta[indiceAtual])
                {
                    
                    indiceAtual++;

                    if (indiceAtual >= ordemCorreta.Length)
                    {
                        Debug.Log("Puzzle completo");
                        indiceAtual = 0;

                        // Libera o item
                        if (rbItem != null)
                        {
                            rbItem.isKinematic = false;
                            rbItem.useGravity = true;
                        }
                    }
                }
                else
                {
                    Debug.Log("Reiniciando sequência");
                    indiceAtual = 0;
                }
            }
        }
    }
}


