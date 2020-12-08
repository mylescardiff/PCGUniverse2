using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace LaireonFramework
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        public enum GradientMode
        {
            Global = 0,
            Local
        }
        public enum GradientDir
        {
            Vertical = 0,
            Horizontal,
            DiagonalLeftToRight,
            DiagonalRightToLeft
            //Free
        }

        public GradientMode gradientMode = GradientMode.Global;
        public GradientDir gradientDir = GradientDir.Vertical;
        public bool overwriteAllColor = false;
        public Color vertex1 = Color.white;
        public Color vertex2 = Color.black;
        private Graphic targetGraphic;

        protected override void Start()
        {
            targetGraphic = GetComponent<Graphic>();
        }

        public enum MeshMode
        {
            Triangles,
            Quads
        }

        public MeshMode mode = MeshMode.Triangles;

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if(!this.IsActive())
                return;

            List<UIVertex> vertexList = new List<UIVertex>();
            vertexHelper.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            if(vertexList.Count == 0)
                return;

            try
            {
                switch(mode)
                {
                    case MeshMode.Triangles:
                        vertexHelper.AddUIVertexTriangleStream(vertexList);
                        break;
                    case MeshMode.Quads:
                        {
                            int quads = vertexList.Count / 4;
                            for(int q = 0; q < quads; q++)
                            {
                                int i = q * 4;
                                vertexHelper.AddUIVertexQuad(new UIVertex[] { vertexList[i + 3], vertexList[i + 2], vertexList[i + 1], vertexList[i] });
                            }
                        }
                        break;
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogWarning("Out of range??? " + ex.Message);
            }
        }

        public void ModifyVertices(List<UIVertex> vertexList)
        {
            if(!IsActive() || vertexList.Count == 0)
                return;

            int count = vertexList.Count;
            UIVertex uiVertex = vertexList[0];

            if(gradientMode == GradientMode.Global)
            {
                if(gradientDir == GradientDir.DiagonalLeftToRight || gradientDir == GradientDir.DiagonalRightToLeft)
                {
#if UNITY_EDITOR
                    Debug.LogError("Diagonal dir is not supported in Global mode");
#endif
                    gradientDir = GradientDir.Vertical;
                }
                float bottomY = gradientDir == GradientDir.Vertical ? vertexList[vertexList.Count - 1].position.y : vertexList[vertexList.Count - 1].position.x;
                float topY = gradientDir == GradientDir.Vertical ? vertexList[0].position.y : vertexList[0].position.x;

                float uiElementHeight = topY - bottomY;

                for(int i = 0; i < count; i++)
                {
                    uiVertex = vertexList[i];
                    if(!overwriteAllColor && uiVertex.color != targetGraphic.color)
                        continue;

                    uiVertex.color *= Color.Lerp(vertex2, vertex1, ((gradientDir == GradientDir.Vertical ? uiVertex.position.y : uiVertex.position.x) - bottomY) / uiElementHeight);
                    vertexList[i] = uiVertex;
                }
            }
            else
            {
                if(targetGraphic == null)
                    return;

                for(int i = 0; i < count; i++)
                {
                    uiVertex = vertexList[i];

                    if(!overwriteAllColor && !CompareCarefully(uiVertex.color, targetGraphic.color))
                        continue;

                    switch(gradientDir)
                    {
                        case GradientDir.Vertical:
                            if(i % 2 == 0)
                                uiVertex.color *= (i % 3 == 0 || (i - 0) % 3 == 0) ? vertex1 : vertex2;
                            else
                                uiVertex.color *= (i % 3 == 0 || (i - 0) % 3 == 0) ? vertex2 : vertex1;
                            break;

                        case GradientDir.Horizontal:
                            if(i % 2 == 0)
                                uiVertex.color *= (i % 3 == 0 || (i - 1) % 3 == 0) ? vertex1 : vertex2;
                            else
                                uiVertex.color *= (i % 3 == 0 || (i - 1) % 3 == 0) ? vertex2 : vertex1;
                            break;

                        case GradientDir.DiagonalLeftToRight:
                            uiVertex.color *= (i % 4 == 0) ? vertex1 : ((i - 2) % 4 == 0 ? vertex2 : Color.Lerp(vertex2, vertex1, 0.55f));
                            break;

                        case GradientDir.DiagonalRightToLeft:
                            uiVertex.color *= ((i - 1) % 4 == 0) ? vertex1 : ((i - 3) % 4 == 0 ? vertex2 : Color.Lerp(vertex2, vertex1, 0.5f));
                            break;

                    }
                    vertexList[i] = uiVertex;
                }
            }
        }

        private bool CompareCarefully(Color col1, Color col2)
        {
            if(Mathf.Abs(col1.r - col2.r) < 0.003f && Mathf.Abs(col1.g - col2.g) < 0.003f && Mathf.Abs(col1.b - col2.b) < 0.003f && Mathf.Abs(col1.a - col2.a) < 0.003f)
                return true;
            return false;
        }
    }
}