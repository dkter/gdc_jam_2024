using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public enum Shape
{
    Circle,
    Line,
    None,
}

public class DrawingIndicator : MonoBehaviour
{
    public Shape shape = Shape.None;
    public Vector3 shapeCentre = Vector3.zero;
    public GameObject gameManager;

    private Mesh mesh;
    private Vector3 lastMousePosition;
    private List<Vector3> mousePositions;

    // Start is called before the first frame update
    void Start()
    {
        mousePositions = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            // mouse button just went down; create new mesh and reset mousePositions list
            mousePositions.Clear();
            mesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[6];

            vertices[0] = GetMousePosition();
            vertices[1] = GetMousePosition();
            vertices[2] = GetMousePosition();
            vertices[3] = GetMousePosition();

            uv[0] = Vector2.zero;
            uv[1] = Vector2.zero;
            uv[2] = Vector2.zero;
            uv[3] = Vector2.zero;

            triangles[0] = 0;
            triangles[1] = 3;
            triangles[2] = 1;

            triangles[3] = 1;
            triangles[4] = 3;
            triangles[5] = 2;

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.MarkDynamic();

            GetComponent<MeshFilter>().mesh = mesh;

            lastMousePosition = GetMousePosition();
            mousePositions.Add(lastMousePosition);
        }

        if (Input.GetMouseButton(0)) {
            // update existing mesh to add length to path
            float minDistance = .1f;
            if (Vector3.Distance(GetMousePosition(), lastMousePosition) > minDistance)
            {
                Vector3[] vertices = new Vector3[mesh.vertices.Length + 2];
                Vector2[] uv = new Vector2[mesh.uv.Length + 2];
                int[] triangles = new int[mesh.triangles.Length + 6];

                mesh.vertices.CopyTo(vertices, 0);
                mesh.uv.CopyTo(uv, 0);
                mesh.triangles.CopyTo(triangles, 0);

                int vIndex = vertices.Length - 4;
                int vIndex0 = vIndex + 0;
                int vIndex1 = vIndex + 1;
                int vIndex2 = vIndex + 2;
                int vIndex3 = vIndex + 3;

                Vector3 mouseForwardVector = (GetMousePosition() - lastMousePosition).normalized;
                Vector3 normal2d = new Vector3(0, 0, -1f);
                float lineThickness = 0.2f;
                Vector3 newVertexUp = GetMousePosition() + Vector3.Cross(mouseForwardVector, normal2d) * lineThickness;
                Vector3 newVertexDown = GetMousePosition() + Vector3.Cross(mouseForwardVector, normal2d * -1f) * lineThickness;

                vertices[vIndex2] = newVertexUp;
                vertices[vIndex3] = newVertexDown;

                uv[vIndex2] = Vector2.zero;
                uv[vIndex3] = Vector2.zero;

                int tIndex = triangles.Length - 6;
                triangles[tIndex + 0] = vIndex0;
                triangles[tIndex + 1] = vIndex2;
                triangles[tIndex + 2] = vIndex1;

                triangles[tIndex + 3] = vIndex1;
                triangles[tIndex + 4] = vIndex2;
                triangles[tIndex + 5] = vIndex3;

                mesh.vertices = vertices;
                mesh.uv = uv;
                mesh.triangles = triangles;

                lastMousePosition = GetMousePosition();
                mousePositions.Add(lastMousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // mouse button just went up, classify shape stored in mousePositions
            double closedLoopThreshold = 1.0;
            if (Vector3.Distance(mousePositions[0], mousePositions[mousePositions.Count - 1]) > closedLoopThreshold)
            {
                this.shape = Shape.Line;
            } else
            {
                this.shape = Shape.Circle;
            }

            Vector3 average = Vector3.zero;
            foreach (Vector3 vec in mousePositions)
            {
                average += vec;
            }
            average = average / mousePositions.Count;
            this.shapeCentre = average;

            gameManager.SendMessage("Summon", this);
        }
    }

    public static Vector3 GetMousePosition()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0; 
        return position;
    }
}
