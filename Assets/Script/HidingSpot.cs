using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    //=============== Intera��o ===============
    [Header("Intera��o")]
    //string
    public string _clsN = "Armario";
    //Game Objects
    public GameObject _p;

    //verifica se o player entrou no arm�rio
    public void OnTriggerEnter(Collider _col)
    {
        //verifica se � o jogador
        if (_col.CompareTag("Player"))
        {
            Debug.Log("Armario Entrado: " + _clsN);
            _p.gameObject.GetComponent<goToPlayer>()._inCls = true;
            //aqui voc� pode adicionar l�gica pra esconder, pontuar, etc
        }
    }
    //verifica se o player saiu do arm�rio
    public void OnTriggerExit(Collider _col)
    {
        //verifica se � o jogador
        if (_col.CompareTag("Player"))
        {
            Debug.Log("Armario Saido: " + _clsN);
            _p.gameObject.GetComponent<goToPlayer>()._inCls = false;
            //aqui voc� pode adicionar l�gica pra esconder, pontuar, etc
        }
    }
}
