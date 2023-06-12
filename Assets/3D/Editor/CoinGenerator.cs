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

    void OnGUI()
    {
        int resolution = 10;
        resolution = EditorGUILayout.IntField(resolution);

        float radius = 1;
        float depth = 0.1f;
        radius = EditorGUILayout.FloatField(radius);
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
        NativeArray<float3> vertices = new((resolution + 1) * 2, Allocator.Temp);
        NativeArray<int> indices = new(resolution * 12, Allocator.Temp);

        int vertexIndex = 0, indexIndex = 0;
        vertices[vertexIndex++] = new float3(0, 0, -depth);
        vertices[vertexIndex++] = new float3(0, 0, depth);
        
        float angle = 0;
        float angleDelta = TAU / resolution;

        int patchStart = vertexIndex;

        float2 circle = new float2(math.cos(angle), math.sin(angle));
        vertices[vertexIndex++] = new float3(circle, -depth);
        vertices[vertexIndex++] = new float3(circle, depth);
        angle += angleDelta;

        for (int i = 0; i < resolution - 1; i++)
        {
            circle = new float2(math.cos(angle), math.sin(angle));
            vertices[vertexIndex++] = new float3(circle, -depth);
            vertices[vertexIndex++] = new float3(circle, depth);
            angle += angleDelta;

            indices[indexIndex++] = 0;
            indices[indexIndex++] = patchStart + 2;
            indices[indexIndex++] = patchStart;

            indices[indexIndex++] = 1;
            indices[indexIndex++] = patchStart + 1;
            indices[indexIndex++] = patchStart + 3;

            int4 quad = new int4(patchStart, patchStart + 2, patchStart + 3, patchStart + 1);
            AddQuad(quad);

            patchStart += 2;
        }

        indices[indexIndex++] = 0;
        indices[indexIndex++] = patchStart;
        indices[indexIndex++] = 2;

        indices[indexIndex++] = 1;
        indices[indexIndex++] = 3;
        indices[indexIndex++] = patchStart + 1;

        int4 lastQuad = new int4(patchStart, 2, 3, patchStart + 1);
        AddQuad(lastQuad);

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

    struct Vertex
    {
        public float3 pos;
        public float2 uv;
    }

    static readonly VertexAttributeDescriptor[] VertexBufferMemoryLayout =
    {
        new VertexAttributeDescriptor(VertexAttribute.Position, stream: 0),
        // new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 0),
        // new VertexAttributeDescriptor(
        //         VertexAttribute.TexCoord0,
        //         VertexAttributeFormat.Float32,
        //         dimension: 2,
        //         stream : 0)
    };
}