using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshRenderer : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        Mesh mesh = new Mesh();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;
        this.GetComponent<MeshFilter>().mesh = mesh;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
