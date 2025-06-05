using UnityEngine; // importa coisas base tipo MonoBehaviour, Transform, etc.

public class goToPlayer : MonoBehaviour // script que move o objeto até outro
{
    //============== REFERÊNCIAS =================
    [Header("Referências")]
    //objects
    public Transform _p; // posição-alvo (outro objeto que ele vai seguir)

    //============== POSIÇÕES ====================
    [Header("Ajustes de Posição")]
    //vetores
    public Vector3 _clsPos; // offset que vai ser somado na posição final
    //bools
    public bool _inCls;

    // roda no inicio do codigo
    public void Start()
    {
        //define o estado como falso
        _inCls = false;

        //define a posição que vai ficar
        _clsPos = new Vector3(0.0f, -50.0f, 0.0f);
    }
    // roda em loop
    public void Update()
    {
        // verifica se o alvo foi setado
        if (_p != null)
        {
            // calcula a posição de destino com o offset
            Vector3 _dstPos = _p.position + _clsPos;

            // verifica se ele ta no armario
            if (_inCls)
            {
                //manda para a posição de destino com o offset
                transform.position = _dstPos;
            }
            // verifica se ele não ta no armario
            else
            {
                //manda para a posição de destino
                transform.position = _p.position;
            }
        }
    }
}
