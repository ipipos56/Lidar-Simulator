using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class UpdateColliderScript : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshCollider meshCollider;
    private Mesh bakedMesh;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        bakedMesh = new Mesh();
    }

    void Update()
    {
        UpdateCollider();
    }

    void UpdateCollider()
    {
        skinnedMeshRenderer.BakeMesh(bakedMesh);
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = bakedMesh;
    }

}
