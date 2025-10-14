using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Configurações")]
    public int _totalPieces = 8;
    private int _placedPieces = 0;

    [Header("Referências")]
    public GameObject _rewardObject;
    public GameObject _incompleteFrame;
    public GameObject _completeFrame;

    [Header("Quadro 3D")]
    public Renderer _frameRenderer;
    public Texture _incompleteTexture;
    public Texture _completeTexture;

    [Header("Recompensa Física")]
    public GameObject _itemLiberado;
    public Rigidbody _rbItem;

    [Header("Organização UI")]
    public Transform _slotsContainer;

    private bool _uvRotated = false;

    void Start()
    {
        if (_totalPieces <= 0)
            _totalPieces = FindObjectsOfType<PuzzlePiece>().Length;

        // ROTACIONA OS UVs DO MESH DO QUADRO (90° para a esquerda)
        RotateFrameMeshUVs90Left();

        if (_frameRenderer != null && _incompleteTexture != null)
            ApplyTexture(_frameRenderer, _incompleteTexture);
    }

    public void PiecePlaced()
    {
        _placedPieces++;
        if (_placedPieces >= _totalPieces)
            PuzzleSolved();
    }

    void PuzzleSolved()
    {
        Debug.Log("Puzzle resolvido!");

        if (_incompleteFrame != null) _incompleteFrame.SetActive(false);
        if (_completeFrame != null) _completeFrame.SetActive(true);

        if (_frameRenderer != null && _completeTexture != null)
            ApplyTexture(_frameRenderer, _completeTexture);

        if (_rewardObject != null) _rewardObject.SetActive(true);

        if (_itemLiberado != null)
        {
            _itemLiberado.SetActive(true);
            if (_rbItem != null)
            {
                _rbItem.isKinematic = false;
                _rbItem.useGravity = true;
            }
        }

        var trigger = GetComponentInParent<PuzzleTrigger>();
        if (trigger != null) trigger.FecharPuzzle();
    }

    private void ApplyTexture(Renderer renderer, Texture tex)
    {
        if (renderer == null || tex == null) return;

        Material mat = renderer.material;

        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", tex);
        else if (mat.HasProperty("_MainTex"))
            mat.SetTexture("_MainTex", tex);
        else
            mat.mainTexture = tex;

        if (mat.HasProperty("_BaseMap"))
        {
            mat.SetTextureScale("_BaseMap", Vector2.one);
            mat.SetTextureOffset("_BaseMap", Vector2.zero);
        }
        else if (mat.HasProperty("_MainTex"))
        {
            mat.SetTextureScale("_MainTex", Vector2.one);
            mat.SetTextureOffset("_MainTex", Vector2.zero);
        }
        else
        {
            mat.mainTextureScale = Vector2.one;
            mat.mainTextureOffset = Vector2.zero;
        }

        if (mat.HasProperty("_Color"))
            mat.color = Color.white;
    }

    private void RotateFrameMeshUVs90Left()
    {
        if (_uvRotated || _frameRenderer == null) return;

        MeshFilter mf = _frameRenderer.GetComponent<MeshFilter>();
        if (mf == null) mf = _frameRenderer.GetComponentInParent<MeshFilter>();

        if (mf != null)
        {
            Mesh mesh = mf.sharedMesh;
            if (mesh != null)
            {
                mesh = Instantiate(mesh);
                mf.mesh = mesh;
            }

            if (mesh != null && mesh.uv != null && mesh.uv.Length > 0)
            {
                Vector2[] oldUV = mesh.uv;
                Vector2[] newUV = new Vector2[oldUV.Length];

                // ROTAÇÃO CORRIGIDA: newU = 1 - oldV ; newV = oldU -> 90° esquerda (CCW)
                for (int i = 0; i < oldUV.Length; i++)
                {
                    Vector2 uv = oldUV[i];
                    newUV[i] = new Vector2(1f - uv.y, uv.x);
                }

                mesh.uv = newUV;
                _uvRotated = true;
                return;
            }
        }

        SkinnedMeshRenderer smr = _frameRenderer.GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
            Mesh shared = smr.sharedMesh;
            if (shared != null)
            {
                Mesh copy = Instantiate(shared);
                if (copy.uv != null && copy.uv.Length > 0)
                {
                    Vector2[] oldUV = copy.uv;
                    Vector2[] newUV = new Vector2[oldUV.Length];
                    for (int i = 0; i < oldUV.Length; i++)
                    {
                        Vector2 uv = oldUV[i];
                        newUV[i] = new Vector2(1f - uv.y, uv.x);
                    }
                    copy.uv = newUV;
                    smr.sharedMesh = copy;
                    _uvRotated = true;
                    return;
                }
            }
        }
    }
}