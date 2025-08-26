using UnityEngine;

public class RandomSound : MonoBehaviour
{
    [Header("Configurações de Áudio")]
    public AudioClip[] sons;          // Sons possíveis
    public float volume = 1f;         // Volume do som
    public int quantidadePorVez = 2;  // Quantos sons tocam de uma vez

    [Header("Configurações de Spawn")]
    public Vector3 areaMin = new Vector3(-10, 0, -10);
    public Vector3 areaMax = new Vector3(10, 0, 10);
    public float intervaloMin = 2f;
    public float intervaloMax = 5f;

    private float proximoTempo;

    void Start()
    {
        DefinirProximoTempo();
    }

    void Update()
    {
        if (Time.time >= proximoTempo)
        {
            ReproduzirSonsAleatorios();
            DefinirProximoTempo();
        }
    }

    void DefinirProximoTempo()
    {
        proximoTempo = Time.time + Random.Range(intervaloMin, intervaloMax);
    }

    void ReproduzirSonsAleatorios()
    {
        if (sons.Length == 0) return;

        for (int i = 0; i < quantidadePorVez; i++)
        {
            // Escolhe um som aleatório
            AudioClip somEscolhido = sons[Random.Range(0, sons.Length)];

            // Define posição aleatória
            float x = Random.Range(areaMin.x, areaMax.x);
            float y = Random.Range(areaMin.y, areaMax.y);
            float z = Random.Range(areaMin.z, areaMax.z);
            Vector3 posicaoAleatoria = new Vector3(x, y, z);

            // Toca o som na posição escolhida
            AudioSource.PlayClipAtPoint(somEscolhido, posicaoAleatoria, volume);
        }
    }
}
