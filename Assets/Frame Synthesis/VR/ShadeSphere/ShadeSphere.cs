using UnityEngine;

// This script is public domain.
// by Katsuomi Kobayashi

namespace FrameSynthesis.VR
{
    public class ShadeSphere : MonoBehaviour
    {
        [Range(-1, 1)]
        public float start = 0.1f;
        [Range(-1, 1)]
        public float end = -0.1f;
        
        public Color shadeColor = Color.black;

        float currentStart;
        float currentEnd;
        Color currentShadeColor;
        
        const int SEGMENTS = 32;
        const int RINGS = 16;
        
        Mesh mesh;
    
        Vector3[] vertices;
        Color[] colors;
        int[] triangles;
        
        void Awake()
        {
            mesh = new Mesh();
            mesh.hideFlags = HideFlags.HideAndDontSave;
            mesh.name = "ShadeSphere (procedual)";

            GetComponent<MeshFilter>().mesh = mesh;
        }

        void OnDisable()
        {
            DestroyImmediate(mesh);
        }

        void Update()
        {
            if (start != currentStart || end != currentEnd || shadeColor != currentShadeColor) {
                // Reconstruct the sphere mesh if updated
                currentStart = start;
                currentEnd = end;
                currentShadeColor = shadeColor;
                CreateSphere();
            }
        }
        
        void CreateSphere()
        {
            int numQuad = SEGMENTS * RINGS;

            mesh.Clear();
            
            vertices = new Vector3[4 * numQuad * 2];
            colors = new Color[4 * numQuad * 2];
            triangles = new int[6 * numQuad * 2];
            
            vertexPos = 0;
            colorPos = 0;
            trianglePos = 0;
            
            for (int ring = 0; ring < RINGS; ring++) {
                for (int segment = 0; segment < SEGMENTS; segment++) {
                    Vector3 v0 = GetVertex(segment    , ring);
                    Vector3 v1 = GetVertex(segment + 1, ring);
                    Vector3 v2 = GetVertex(segment    , ring + 1);
                    Vector3 v3 = GetVertex(segment + 1, ring + 1);
                    
                    CreateQuad(v0, v1, v2, v3);
                }
            }
    
            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.triangles = triangles;
        }
        
        int vertexPos;
        int colorPos;
        int trianglePos;
        
        void CreateQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Color c0 = Z2Color(v0.z);
            Color c1 = Z2Color(v1.z);
            Color c2 = Z2Color(v2.z);
            Color c3 = Z2Color(v3.z);
            
            if (c0.a == 0f &&
                c1.a == 0f &&
                c2.a == 0f &&
                c3.a == 0f) {
                // Discard invisible polygon
                return;
            }
            
            vertices[vertexPos    ] = v0;
            vertices[vertexPos + 1] = v1;
            vertices[vertexPos + 2] = v2;
            vertices[vertexPos + 3] = v3;
            
            colors[colorPos    ] = c0;
            colors[colorPos + 1] = c1;
            colors[colorPos + 2] = c2;
            colors[colorPos + 3] = c3;
            
            triangles[trianglePos    ] = vertexPos + 2;
            triangles[trianglePos + 1] = vertexPos + 0;
            triangles[trianglePos + 2] = vertexPos + 1;
            triangles[trianglePos + 3] = vertexPos + 1;
            triangles[trianglePos + 4] = vertexPos + 3;
            triangles[trianglePos + 5] = vertexPos + 2;
            
            vertexPos += 4;
            colorPos += 4;
            trianglePos += 6;
        }
        
        Vector3 GetVertex(int segment, int ring)
        {
            float lng = 2f * Mathf.PI * segment / SEGMENTS;
            float lat = 360f * ring / RINGS;
            
            Vector3 v = new Vector3(Mathf.Cos(lng), 0f, Mathf.Sin(lng));
            
            return Quaternion.AngleAxis(lat, Vector3.forward) * v;
        }
        
        Color Z2Color(float z)
        {
            float alpha = LinearClampedMap(z, start, end, 0f, 1f);
            
            Color color = shadeColor;
            color.a = alpha;
            
            return color;
        }
        
        float LinearMap(float value, float s0, float s1, float d0, float d1)
        {
            return d0 + (value - s0) * (d1 - d0) / (s1 - s0);
        }
        
        float LinearClampedMap(float value, float s0, float s1, float d0, float d1)
        {
            if (d0 < d1) {
                return Mathf.Clamp(LinearMap(value, s0, s1, d0, d1), d0, d1);
            }
            return Mathf.Clamp(LinearMap(value, s0, s1, d0, d1), d1, d0);
        }
    }
}


