using System.Collections.Generic;
using UnityEngine;

public class SineWaveInstanced : MonoBehaviour {
    #region Fields

    public GameObject sample;
    public Mesh mesh;
    public Material material;

    public int gridSize = 40;
    public float spacing = 1.5f;
    public float waveSpeed = 2f;
    public float waveHeight = 0.5f;
    public float waveFrequency = 0.5f;

    RenderParams rp;
    InstanceData instance;

    public struct InstanceData {
        public Matrix4x4 objectToWorld;
        public uint renderingLayerMask;
        public float distance;
    }

    List<InstanceData> instances = new ();

    float gridCenterX;
    float gridCenterZ;

    #endregion

    void Start() {
        // If a sample GameObject is defined, grab its mesh
        if (sample != null) {
            mesh = sample.GetComponent<MeshFilter>().sharedMesh;
        }

        // If there’s no mesh, create a primitive sphere as a fallback
        if (mesh == null) {
            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mesh = primitive.GetComponent<MeshFilter>().sharedMesh;
            Destroy(primitive);
        }

        // Enable GPU instancing in the material
        material.enableInstancing = true;

        // Configure rendering parameters
        rp = new RenderParams(material);
        rp.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rp.receiveShadows = false;

        // Calculate grid center positions
        gridCenterX = gridSize * spacing * 0.5f;
        gridCenterZ = gridSize * spacing * 0.5f;

        // Prepare the list of instance data
        for (int x = 0; x < gridSize; x++)
        for (int z = 0; z < gridSize; z++) {
            Vector3 pos = new (x * spacing, 0, z * spacing);
            instances.Add(
                new InstanceData {
                    objectToWorld = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one),
                    distance = Vector3.Distance(pos, new Vector3(gridCenterX, 0, gridCenterZ))
                }
            );
        }
    }

    void Update() {
        float time = Time.time * waveSpeed;

        // Update instance data with wave transformations
        for (int i = 0; i < instances.Count; i++) {
            instance = instances[i];

            float yOffset = Mathf.Sin(time - instance.distance * waveFrequency) * waveHeight;

            // [ Right.x, Up.x, Forward.x, Position.x ]   // First row
            // [ Right.y, Up.y, Forward.y, Position.y ]   // Second row
            // [ Right.z, Up.z, Forward.z, Position.z ]   // Third row
            // [   0    ,   0 ,    0     ,      1     ]   // Fourth row, typically used for scaling factor or perspective            

            // Matrix transformation for wave animation
            instance.objectToWorld.m13 = yOffset; // Set Y-position
            instance.renderingLayerMask = 1u;

            instances[i] = instance; // Update instance in the list
        }

        // Render instanced meshes
        Graphics.RenderMeshInstanced(rp, mesh, 0, instances);
    }
}