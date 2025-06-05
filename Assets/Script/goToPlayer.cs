using UnityEngine; // importa coisas base tipo MonoBehaviour, Transform, etc.

public class goToPlayer : MonoBehaviour // script que move o objeto at� outro
{
    //============== REFER�NCIAS =================
    [Header("Refer�ncias")]
    //objects
    public Transform _p; // posi��o-alvo (outro objeto que ele vai seguir)

    //============== POSI��ES ====================
    [Header("Ajustes de Posi��o")]
    //vetores
    public Vector3 _clsPos; // offset que vai ser somado na posi��o final
    //bools
    public bool _inCls;

    // roda no inicio do codigo
    public void Start()
    {
        //define o estado como falso
        _inCls = false;

        //define a posi��o que vai ficar
        _clsPos = new Vector3(0.0f, -50.0f, 0.0f);
    }
    // roda em loop
    public void Update()
    {
        // verifica se o alvo foi setado
        if (_p != null)
        {
            // calcula a posi��o de destino com o offset
            Vector3 _dstPos = _p.position + _clsPos;

            // verifica se ele ta no armario
            if (_inCls)
            {
                //manda para a posi��o de destino com o offset
                transform.position = _dstPos;
            }
            // verifica se ele n�o ta no armario
            else
            {
                //manda para a posi��o de destino
                transform.position = _p.position;
            }
        }
    }
}
