#define StoreVersion
#define UsingUGUI
//#define UsingNGUI

using UnityEngine;
using System.Collections;

namespace TransitionalObjects
{
    [AddComponentMenu("")]//hides this script from being added
    public class ColourTransition : BaseTransition
    {
        public Color startColour, endColour;

        protected override void Transition(float transitionPercentage)
        {
#if(StoreVersion)
            SetColour(TransitionalObject.Lerp(startColour, endColour, transitionPercentage));
#else
            SetColour(K2Maths.Lerp(startColour, endColour, transitionPercentage));
#endif
        }

        void SetColour(Color colour)
        {
#if(UsingNGUI)
            for(int i = 0; i < parent.affectedWidgets.Length; i++)
                parent.affectedWidgets[i].color = colour;
#endif

#if(UsingUGUI)
            for(int i = 0; i < parent.affectedImages.Length; i++)
                parent.affectedImages[i].color = colour;

            for(int i = 0; i < parent.affectedCanvasGroups.Length; i++)
                parent.affectedCanvasGroups[i].alpha = colour.a;
#endif

#if(UNITY_EDITOR)
            if(Application.isEditor)//is the editor running
            {
                for(int i = 0; i < parent.affectedRenderers.Length; i++)
                    parent.affectedRenderers[i].sharedMaterial.color = colour;
            }
            else
            {
#endif
                for(int i = 0; i < parent.affectedRenderers.Length; i++)
                    if(parent.affectedRenderers.GetType().Equals(typeof(SpriteRenderer)))
                        ((SpriteRenderer)parent.affectedRenderers[i]).color = colour;
                    else
                        for(int ii = 0; ii < parent.affectedRenderers[i].materials.Length; ii++)
                            parent.affectedRenderers[i].materials[ii].color = colour;

#if(UNITY_EDITOR)
            }
#endif
        }

        public override void Clone(BaseTransition other)
        {
            base.Clone(other);

            ColourTransition converted = (ColourTransition)other;

            startColour = converted.startColour;
            endColour = converted.endColour;
        }

        #region Editor Externals
#if(UNITY_EDITOR)
        /// <summary>
        /// Called by the editor to view either the start of end point
        /// </summary>
        public override void ViewPosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
                SetColour(startColour);
            else
                SetColour(endColour);
        }

        /// <summary>
        /// Called by the editor to update the start and end points based on the current position
        /// </summary>
        /// <param name="isStartPoint"></param>
        public override void UpdatePosition(TransitionalObject.MovingDataType movingType)
        {
        }

        public override void SwapDataFields()
        {
            Color temp = startColour;

            startColour = endColour;
            endColour = temp;
        }
#endif
        #endregion
    }
}