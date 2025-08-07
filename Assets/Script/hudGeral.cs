using UnityEngine;
using UnityEngine.UI;

public class hudGeral : MonoBehaviour
{
    //referência pro player que tem a variável _lntrOn
    [Header("Referência")]
    public GameObject _player; //player
    public GameObject _lanterna;

    //sprites da lanterna (posição 0 = desligada, 1 = ligada)
    [Header("Sprites da Lanterna")]
    public Sprite[] _spriteDaLanterna;

    public void Update()
    {
        //verifica se a lanterna do player está ligada
        if (_player.GetComponent<Player>()._lntrOn)
        {
            //sprite ligada
            _lanterna.GetComponent<Image>().sprite = _spriteDaLanterna[1];
        }
        else
        {
            //sprite desligada
            _lanterna.GetComponent<Image>().sprite = _spriteDaLanterna[0];
        }
    }
}
