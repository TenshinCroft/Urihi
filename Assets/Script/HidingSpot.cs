using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    //=============== Interação ===============
    [Header("Interação")]
    //string
    public string _clsN = "Armario";
    //Game Objects
    public GameObject _p;

    //verifica se o player entrou no armário
    public void OnTriggerEnter(Collider _col)
    {
        //verifica se é o jogador
        if (_col.CompareTag("Player"))
        {
            Debug.Log("Armario Entrado: " + _clsN);
            _p.gameObject.GetComponent<goToPlayer>()._inCls = true;
            //aqui você pode adicionar lógica pra esconder, pontuar, etc
        }
    }
    //verifica se o player saiu do armário
    public void OnTriggerExit(Collider _col)
    {
        //verifica se é o jogador
        if (_col.CompareTag("Player"))
        {
            Debug.Log("Armario Saido: " + _clsN);
            _p.gameObject.GetComponent<goToPlayer>()._inCls = false;
            //aqui você pode adicionar lógica pra esconder, pontuar, etc
        }
    }
}
