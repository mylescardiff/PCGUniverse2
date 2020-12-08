// ------------------------------------------------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 6/15/2020
// ------------------------------------------------------------------------------

using UnityEngine;

namespace PcgUniverse2
{

/// <summary>
/// Represents a single side of the icosphere in a planet's mesh
/// </summary>
public class TerrainFace
{
    private Planet m_planet;
    private Mesh m_mesh;
    private int m_resolution;
    private Vector3 m_localUp;

    private Vector3 m_axisA;
    private Vector3 m_axisB;

    /// <summary>
    /// Constructor
    /// </summary>
    public TerrainFace(Planet planet, Mesh mesh, int resolution, Vector3 localUp)
    {
        m_planet = planet;
        m_mesh = mesh;
        m_resolution = resolution;
        m_localUp = localUp;

        m_axisA = new Vector3(m_localUp.y, m_localUp.z, m_localUp.x);
        m_axisB = Vector3.Cross(m_localUp, m_axisA);

    }

    /// <summary>
    /// Calculate the vertices and triangles for a single side of a planet mesh
    /// </summary>
    public void ConstructMesh()
    {
        Planet.MeshData meshData = new Planet.MeshData();

        // set up the raw data for the mesh
        meshData.m_vertices = new Vector3[m_resolution * m_resolution];
        meshData.m_triangles = new int[(m_resolution - 1) * (m_resolution - 1) * 6];

        // store this becuase it can be saved and reapplied to the mesh after update
        // calculate UVS; added a check here to make sure its the right size, becauase
        // scaling the planet's resolution in the editor fucks it up.
        meshData.m_uvs = new Vector2[meshData.m_vertices.Length];

        int triangleIndex = 0;
        for (int y = 0; y < m_resolution; y++)
        {
            for (int x = 0; x < m_resolution; x++)
            {
                int vertexIndex = x + y * m_resolution;
                // percent tells how far along this loop is, and hence how 
                // far up the vertex should be along the face. 
                Vector2 percent = new Vector2(x, y) / (m_resolution - 1);

                // our mesh starts out as a cube with 6 faces
                // we need a value between 0 and 1 to tell how far up the axes 
                Vector3 pointOnUnitCube = m_localUp + (percent.x - 0.5f) * 2 * m_axisA + (percent.y - 0.5f) * 2 * m_axisB;

                // normalizing here pushes these vectors all out to a magnitude of 1, creating a sphere
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                meshData.m_vertices[vertexIndex] = m_planet.CalculatePointOnPlanet(pointOnUnitSphere) + m_planet.transform.position;

                // don't create triabgles along the bottom or left edge, becuase
                // those would be outside the mesh
                if (x < m_resolution - 1 && y < m_resolution - 1)
                {
                    // triangle 1
                    meshData.m_triangles[triangleIndex++] = vertexIndex;
                    meshData.m_triangles[triangleIndex++] = vertexIndex + m_resolution + 1;
                    meshData.m_triangles[triangleIndex++] = vertexIndex + m_resolution;

                    // triangle 2
                    meshData.m_triangles[triangleIndex++] = vertexIndex;
                    meshData.m_triangles[triangleIndex++] = vertexIndex + 1;
                    meshData.m_triangles[triangleIndex++] = vertexIndex + m_resolution + 1;
                }
            }
        }

        m_mesh.Clear();
        m_mesh.vertices = meshData.m_vertices;
        m_mesh.triangles = meshData.m_triangles;
        m_mesh.RecalculateNormals();
        m_mesh.uv = meshData.m_uvs;

    }

    /// <summary>
    /// Updates the UVs when the planet data changes, e.g. when the resolution changes 
    /// and we now have a different number of vertices
    /// </summary>
    public void UpdateUVs()
    {
        Vector2[] uvs = new Vector2[m_resolution * m_resolution];

        for (int y = 0; y < m_resolution; y++)
        {
            for (int x = 0; x < m_resolution; x++)
            {
                int vertexIndex = x + y * m_resolution;
                Vector2 percent = new Vector2(x, y) / (m_resolution - 1);
                Vector3 pointOnUnitCube = m_localUp + (percent.x - 0.5f) * 2 * m_axisA + (percent.y - 0.5f) * 2 * m_axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                uvs[vertexIndex] = new Vector2(m_planet.BiomePercentFromPoint(pointOnUnitSphere), 0);
            }
        }

        // resolution change will F this up
        if (m_mesh.uv.Length != m_mesh.vertexCount)
            m_mesh.uv = new Vector2[m_mesh.vertexCount];

        m_mesh.uv = uvs;
    }
}

}
