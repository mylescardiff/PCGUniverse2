#define UsingUGUI
//#define UsingNGUI

using UnityEngine;
using System.Collections;

namespace TransitionalObjects
{
    [AddComponentMenu("")]//hides this script from being added
    [System.Serializable]
    public class AlphaTransition : BaseTransition
    {
        public bool startFaded;
        Material[][] materials;//garbage sink. Renderer.materials returns a Copy of the materials! Not a reference which is nasty as hell

        public override void Initialise()
        {
            if(startFaded)
            {
                currentTime = 0;//start at nothing
                Transition();//set the values
            }

            base.Initialise();
        }

        protected override void Transition(float transitionPercentage)
        {
            InitialiseMaterials();

#if(UsingNGUI)
            for(int i = 0; i < parent.affectedWidgets.Length; i++)
                parent.affectedWidgets[i].alpha = transitionPercentage;
#endif

#if(UsingUGUI)
            for(int i = 0; i < parent.affectedImages.Length; i++)
                parent.affectedImages[i].color = new Color(parent.affectedImages[i].color.r, parent.affectedImages[i].color.g, parent.affectedImages[i].color.b, transitionPercentage * parent.childrenMaxAlpha[parent.imageStartIndex + i]);

            for(int i = 0; i < parent.affectedCanvasGroups.Length; i++)
                parent.affectedCanvasGroups[i].alpha = transitionPercentage * parent.childrenMaxAlpha[parent.imageStartIndex + i];
#endif

            for(int i = 0; i < parent.affectedRenderers.Length; i++)
                if(parent.affectedRenderers.GetType().Equals(typeof(SpriteRenderer)))
                    ((SpriteRenderer)parent.affectedRenderers[i]).color = new Color(((SpriteRenderer)parent.affectedRenderers[i]).color.r, ((SpriteRenderer)parent.affectedRenderers[i]).color.g, ((SpriteRenderer)parent.affectedRenderers[i]).color.b, transitionPercentage * parent.childrenMaxAlpha[i]);
                else
                    for(int ii = 0; ii < materials[i].Length; ii++)
                        materials[i][ii].color = new Color(materials[i][ii].color.r, materials[i][ii].color.g, materials[i][ii].color.b, transitionPercentage * parent.childrenMaxAlpha[i]);
        }

        void InitialiseMaterials()
        {
            if(materials == null)
            {
                materials = new Material[parent.affectedRenderers.Length][];

                for(int i = 0; i < parent.affectedRenderers.Length; i++)
                    materials[i] = parent.affectedRenderers[i].materials;
            }
        }

        public override void Clone(BaseTransition other)
        {
            base.Clone(other);

            startFaded = ((AlphaTransition)other).startFaded;
        }

        #region Editor Externals
#if(UNITY_EDITOR)
        /// <summary>
        /// Called by the editor to update the start and end points based on the current position
        /// </summary>
        /// <param name="isStartPoint"></param>
        public override void UpdatePosition(TransitionalObject.MovingDataType movingType)
        {
        }

        /// <summary>
        /// Called by the editor to view either the start of end point
        /// </summary>
        public override void ViewPosition(TransitionalObject.MovingDataType movingType)
        {
        }
#endif
        #endregion
    }
}