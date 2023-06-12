using System.Runtime.InteropServices;

using UnityEditor;
using UnityEngine;

using UnityEngine.Rendering;

using Unity.Collections;
using Unity.Mathematics;

public class CoinGenerator : EditorWindow
{
    [MenuItem("Sunday/CoinGenerator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CoinGenerator));
    }

    int resolution = 10;
    float radius = 1;
    float depth = 0.1f;

    void OnGUI()
    {
        EditorGUILayout.LabelField("Resolution");
        resolution = EditorGUILayout.IntField(resolution);

        EditorGUILayout.LabelField("Radius");
        radius = EditorGUILayout.FloatField(radius);
        EditorGUILayout.LabelField("Depth");
        depth = EditorGUILayout.FloatField(depth);

        string path = "Assets/3D/Coin.asset";
        path = EditorGUILayout.TextField(path);
        if (GUILayout.Button("Generate"))
        {
            Mesh mesh = CreateCoinMesh(resolution, radius, depth);
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
        }
    }

    const float TAU = math.PI * 2;

    Mesh CreateCoinMesh(int resolution, float radius, float depth)
    {
        NativeArray<Vertex> vertices = new((resolution + 1) * 2 + (resolution + 1) * 4, Allocator.Temp);
        NativeArray<int> indices = new(resolution * 3 * 2 + (resolution + 1) * 6, Allocator.Temp);

        int vertexIndex = 0, indexIndex = 0;
        CreateObverseOrReverse(true, out int obverseStartV);
        CreateObverseOrReverse(false, out int reverseStartV);
        CreateEdge(obverseStartV, reverseStartV);

        void CreateObverseOrReverse(bool isObverse, out int startV)
        {
            float z = isObverse ? -depth : depth;
            
            int centerV = vertexIndex;
            float2 uv = isObverse ? new float2(0.25f, 0.25f) : new float2(0.25f, 0.75f);
            float3 normal = isObverse ? new float3(0, 0, -1) : new float3(0, 0, 1);
            AddVertex(new float3(0, 0, z), normal, uv);

            float angle = 0;
            float angleDelta = TAU / resolution;

            float2 circle = new float2(math.cos(angle), math.sin(angle));

            int prevV = vertexIndex;
            startV = vertexIndex;
            int currentV = -1;
            AddVertex(new float3(circle * radius, z), normal, GetObverseOrReverseUv(circle, isObverse));
            angle += angleDelta;
            
            for (int i = 0; i < resolution; i++)
            {
                if (i != resolution - 1)
                { 
                    circle = new float2(math.cos(angle), math.sin(angle));
                    currentV = vertexIndex;
                    AddVertex(new float3(circle * radius, z), normal, GetObverseOrReverseUv(circle, isObverse));
                    angle += angleDelta;
                }
                else
                {
                    currentV = startV;
                }

                if (isObverse)
                { 
                    indices[indexIndex++] = centerV;
                    indices[indexIndex++] = currentV;
                    indices[indexIndex++] = prevV;
                }
                else
                { 
                    indices[indexIndex++] = centerV;
                    indices[indexIndex++] = prevV;
                    indices[indexIndex++] = currentV;
                }

                prevV = currentV;
            }
        }

        void CreateEdge(int obverseV, int reverseV)
        {
            int startObverseV = obverseV;
            int startReverseV = reverseV;
            /// Reuse positions from obverse and reverse, and modify only uv
            for (int i = 0; i < resolution; i++)
            {
                float2 uv = i % 2 == 0 ? GetEvenEdgeUv() : GetOddEdgeUv();

                int q = vertexIndex;
                float3x4 pos = new float3x4(
                    vertices[obverseV++].pos,
                    vertices[reverseV++].pos,
                    vertices[reverseV].pos,
                    vertices[obverseV].pos
                );
                if (i == resolution - 1)
                {
                    pos[2] = vertices[startReverseV].pos;
                    pos[3] = vertices[startObverseV].pos;
                }
                float3 normal = new float3((pos[1].xy + pos[2].xy) / 2, 0);
                
                AddVertex(pos[0], normal, uv);
                AddVertex(pos[1], normal, uv);
                AddVertex(pos[2], normal, uv);
                AddVertex(pos[3], normal, uv);

                AddQuad(new int4(q + 3, q + 2, q + 1, q));
            }
        }

        void AddVertex(float3 pos, float3 normal, float2 uv)
        {
            vertices[vertexIndex++] = new Vertex()
            {
                pos = pos,
                normal = normal,
                uv = uv
            };
        }

        float2 GetObverseOrReverseUv(float2 circle, bool isObverse)
        {
            return isObverse ? circle / 4 + new float2(-0.25f, -0.25f) + 0.5f : circle / 4 + new float2(-0.25f, 0.25f) + 0.5f;
        }

        float2 GetEvenEdgeUv()
        {
            return new float2(0.75f, 0.25f);
        }

        float2 GetOddEdgeUv()
        {
            return new float2(0.75f, 0.75f);
        }

        void AddQuad(int4 quad)
        {
            indices[indexIndex++] = quad.x;
            indices[indexIndex++] = quad.y;
            indices[indexIndex++] = quad.z;
            indices[indexIndex++] = quad.x;
            indices[indexIndex++] = quad.z;
            indices[indexIndex++] = quad.w;
        }

        Mesh mesh = new Mesh();

        mesh.SetVertexBufferParams(vertices.Length, VertexBufferMemoryLayout);
        mesh.SetIndexBufferParams(indices.Length, IndexFormat.UInt32);

        mesh.SetVertexBufferData(vertices, 0, 0, vertices.Length, 0, GenerationMeshUpdateFlags);
        mesh.SetIndexBufferData(indices, 0, 0, indices.Length, GenerationMeshUpdateFlags);

        mesh.subMeshCount = 1;
        SubMeshDescriptor subMesh = new SubMeshDescriptor(
            indexStart: 0,
            indexCount: indices.Length
        );
        mesh.SetSubMesh(0, subMesh);

        mesh.RecalculateBounds();
        return mesh;
    }

    const MeshUpdateFlags GenerationMeshUpdateFlags = MeshUpdateFlags.Default;

    [StructLayout(LayoutKind.Sequential)]
    struct Vertex
    {
        public float3 pos;
        public float3 normal;
        public float2 uv;
    }

    static readonly VertexAttributeDescriptor[] VertexBufferMemoryLayout =
    {
        new VertexAttributeDescriptor(VertexAttribute.Position, stream: 0),
        new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 0),
        new VertexAttributeDescriptor(
                VertexAttribute.TexCoord0,
                VertexAttributeFormat.Float32,
                dimension: 2,
                stream : 0)
    };
}