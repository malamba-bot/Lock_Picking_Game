/* THIS SCRIPT IS NOT USED, but left in the project for future reference */
using UnityEngine;

public class HalfCircle : MonoBehaviour {
    [SerializeField] private int segments = 32;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float radians = Mathf.PI;

    private void Start() {
        GetComponent<MeshFilter>().mesh = BuildMesh();
    }

    private Mesh BuildMesh() {
        Mesh mesh = new Mesh();

        int vertCount = segments + 2; // center + arc points
        Vector3[] verts = new Vector3[vertCount];
        int[] tris = new int[segments * 3];

        //verts[0] = Vector3.zero; // center
        verts[0] = new Vector3 (0, 1, 0); 

        /*  Place each vertice of the mesh in a semi-circle */
        for (int i = 0; i <= segments; i++) {
            float angle = radians * i / segments; // 0 to PI = half circle
            verts[i + 1] = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0);
        }

        /* Tris holds indexes which it uses to access the verts array */
        for (int i = 0; i < segments; i++) {
            tris[i * 3]     = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = i + 2;
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }
}
