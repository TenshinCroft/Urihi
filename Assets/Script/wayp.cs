using UnityEngine;

public class GizmoEsfera : MonoBehaviour
{
    public float raio = 2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, raio);
    }
}
