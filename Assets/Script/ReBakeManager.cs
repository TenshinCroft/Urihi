using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ReBakeManager : MonoBehaviour
{
    public NavMeshSurface navSurface;

    public void RebakeNavMesh()
    {
        navSurface.BuildNavMesh();
    }
}
