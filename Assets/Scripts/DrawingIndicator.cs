using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public enum Shape
{
    Circle,
    Line,
    // Triangle,
    Rectangle,
    // Diamond,
    None,
}

public class DrawingIndicator : MonoBehaviour
{
    public Shape shape = Shape.None;
    public Vector3 shapeCentre = Vector3.zero;
    public Vector3 shapeVector = Vector3.zero;
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


            //if (boundingRectArea < areaThreshold)
            //{
            //    this.shape = Shape.None;
            //}
            //else if (p2a_ratio )
            ////else if (Vector3.Distance(mousePositions[0], mousePositions[mousePositions.Count - 1]) > closedLoopThreshold)
            //else if (p2a_ratio > 55)
            //{
            //    this.shape = Shape.Line;
            //    this.shapeVector = mousePositions[mousePositions.Count - 1] - mousePositions[0];
            //}
            ////else if (Mathf.Abs(width - height) < width_height_threshold)
            //else if (area_ratio > 95)
            //{
            //    this.shape = Shape.Rectangle;
            //    this.shapeVector = Vector3.zero;
            //}
            //else if (area_ratio > 70)
            //{
            //    this.shape = Shape.Circle;
            //    this.shapeVector = Vector3.zero;
            //}

            //else
            //{
            //    this.shape = Shape.None;
            //}
            this.shape = ClassifyShape();
            this.shapeVector = mousePositions[mousePositions.Count - 1] - mousePositions[0];

            gameManager.SendMessage("Summon", this);

            // make shape disappear
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    public Shape ClassifyShape()
    {
        Vector3 average = Vector3.zero;
        float maxX = float.NegativeInfinity;
        float maxY = float.NegativeInfinity;
        float minX = float.PositiveInfinity;
        float minY = float.PositiveInfinity;
        foreach (Vector3 vec in mousePositions)
        {
            average += vec;
            if (vec.x < minX) minX = vec.x;
            if (vec.x > maxX) maxX = vec.x;
            if (vec.y < minY) minY = vec.y;
            if (vec.y > maxY) maxY = vec.y;
        }
        average = average / mousePositions.Count;
        this.shapeCentre = average;

        // line detection:
        // if the ends are close enough to x/y extrema then it's a line
        var line_x1_left_closeness = (mousePositions[0].x - minX) / (maxX - minX);
        var line_xn_right_closeness = (maxX - mousePositions[mousePositions.Count - 1].x) / (maxX - minX);
        var line_x1_right_closeness = (maxX - mousePositions[0].x) / (maxX - minX);
        var line_xn_left_closeness = (mousePositions[mousePositions.Count - 1].x - minX) / (maxX - minX);
        var line_y1_top_closeness = (mousePositions[0].y - minY) / (maxY - minY);
        var line_yn_bottom_closeness = (maxY - mousePositions[mousePositions.Count - 1].y) / (maxY - minY);
        var line_y1_bottom_closeness = (maxY - mousePositions[0].y) / (maxY - minY);
        var line_yn_top_closeness = (mousePositions[mousePositions.Count - 1].y - minY) / (maxY - minY);
        var closeness_threshold = 0.05;
        if (
            (line_x1_left_closeness < closeness_threshold && line_xn_right_closeness < closeness_threshold)
            || (line_x1_right_closeness < closeness_threshold && line_xn_left_closeness < closeness_threshold)
            || (line_y1_top_closeness < closeness_threshold && line_yn_bottom_closeness < closeness_threshold)
            || (line_y1_bottom_closeness < closeness_threshold && line_yn_top_closeness < closeness_threshold)
        )
        {
            return Shape.Line;
        }

        float width = maxX - minX;
        float height = maxY - minY;
        float w_h = width / height;
        float width_height_threshold = 2F;

        var convexHull = GetConvexHull();
        float perimeter = GetPolygonPerimeter(convexHull);
        float area = GetPolygonArea(convexHull);
        float boundingRectArea = (maxX - minX) * (maxY - minY);
        float p2a_ratio = perimeter * perimeter / area;
        float area_ratio = area / boundingRectArea;

        var p2a_square = 16;
        var p2a_rect = P2aRect(w_h);
        // var p2a_triangle = P2aTriangle(w_h);
        // var p2a_diamond = P2aDiamond(w_h);
        var p2a_ellipse = P2aEllipse(w_h);
        var p2a_ideal = float.PositiveInfinity;
        var closest_shape = Shape.None;
        if (Mathf.Abs(p2a_ratio - p2a_square) < Mathf.Abs(p2a_ratio - p2a_ideal)) { p2a_ideal = p2a_square; closest_shape = Shape.Rectangle; }
        if (Mathf.Abs(p2a_ratio - p2a_rect) < Mathf.Abs(p2a_ratio - p2a_ideal)) { p2a_ideal = p2a_rect; closest_shape = Shape.Rectangle; }
        // if (Mathf.Abs(p2a_ratio - p2a_triangle) < Mathf.Abs(p2a_ratio - p2a_ideal)) { p2a_ideal = p2a_triangle; closest_shape = Shape.Triangle; }
        // if (Mathf.Abs(p2a_ratio - p2a_diamond) < Mathf.Abs(p2a_ratio - p2a_ideal)) { p2a_ideal = p2a_diamond; closest_shape = Shape.Diamond; }
        if (Mathf.Abs(p2a_ratio - p2a_ellipse) < Mathf.Abs(p2a_ratio - p2a_ideal)) { p2a_ideal = p2a_ellipse; closest_shape = Shape.Circle; }

        // Debug.Log("perim: " + perimeter);
        // Debug.Log("area: " + area);
        // Debug.Log("p2a: " + p2a_ratio);
        // Debug.Log("area ratio: " + area_ratio);
        // Debug.Log($"convex hull:\n{string.Join("\n", convexHull)}");

        if (p2a_ratio < 4*Mathf.PI + 2)
        {
            return Shape.Circle;
        }
        else if (p2a_ratio > 55)
        {
            return Shape.Line;
        }
        // else if (0.45 < area_ratio && area_ratio < 0.55)
        // {
        //     if (Mathf.Abs(p2a_ratio - p2a_triangle) < Mathf.Abs(p2a_ratio - p2a_diamond))
        //         return Shape.Triangle; // triangle or diamond
        //     else
        //         return Shape.Diamond;
        // }
        else
        {
            if (closest_shape == Shape.Rectangle || closest_shape == Shape.Circle)
            {
                return closest_shape;
            }
        }

        return Shape.None;
    }

    public static Vector3 GetMousePosition()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0; 
        return position;
    }

    public List<Vector3> GetConvexHull() {
        List<Vector3> hull = new List<Vector3>();

        // get leftmost point
        Vector3 leftmost = mousePositions[0];
        foreach (Vector3 vec in mousePositions) {
            if (vec.x < leftmost.x) {
                leftmost = vec;
            }
        }

        Vector3 endpoint = leftmost;
        do
        {
            Vector3 lastPoint = endpoint;
            hull.Add(endpoint);
            endpoint = mousePositions[0];
            foreach (Vector3 vec in mousePositions)
            {
                if (endpoint == lastPoint || (
                    (endpoint.x - lastPoint.x) * (vec.y - lastPoint.y) - (endpoint.y - lastPoint.y) * (vec.x - lastPoint.x) > 0
                ))
                {
                    endpoint = vec;
                }
            }
        } while (endpoint != leftmost);

        return hull;
    }

    public static float GetPolygonArea(List<Vector3> vertices)
    {
        var result = vertices[vertices.Count - 1].x * vertices[0].y - vertices[vertices.Count - 1].y * vertices[0].x;
        for (int i = 1; i < vertices.Count; i += 1)
            result += vertices[i - 1].x * vertices[i].y - vertices[i - 1].y * vertices[i].x;

        return Mathf.Abs(result * .5f);
    }

    public static float GetPolygonPerimeter(List<Vector3> vertices)
    {
        var result = Mathf.Abs(Vector3.Distance(vertices[vertices.Count - 1], vertices[0]));
        for (int i = 1; i < vertices.Count; i += 1)
            result += Mathf.Abs(Vector3.Distance(vertices[i - 1], vertices[i]));

        return result;
    }

    public static float P2aRect(float w_h)
    {
        return 4 * (w_h + 2 + 1 / w_h);
    }

    public static float P2aTriangle(float w_h)
    {
        return 2 * Mathf.Pow(w_h + Mathf.Sqrt(w_h * w_h + 4), 2);
    }

    public static float P2aDiamond(float w_h)
    {
        return 8 * (w_h * w_h + 1) / w_h / w_h;
    }

    public static float P2aEllipse(float w_h)
    {
        float K = 0.005095f * Mathf.Pow(w_h, 4) - 0.0693346f * Mathf.Pow(w_h, 3)
            - 0.519223f * w_h * w_h + 0.346653f * w_h + 0.24308f;
        return Mathf.PI * ((w_h + 1) / 2) - K;
    }
}
