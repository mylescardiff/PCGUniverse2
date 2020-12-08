using UnityEngine;
using System.Collections;

namespace TransitionalObjects
{
    [AddComponentMenu("")]//hides this script from being added
    public class MovingTransition : BaseTransition
    {
        [HideInInspector]
        public new Transform transform;

        public enum MovementType { Absolute = 0, Local, Difference, RectTransform }//basically do we move from exactly one position to another, from one local position to another, or do we read the end point and move in relation
        public MovementType type = MovementType.Local;

        public bool deviateStart, deviateEnd, startAtCurrent, endAtCurrent;//use this to select between to random positions

        public Vector3 differenceStartPoint, startPoint, endPoint;
        public Vector3 minStart, maxStart, minEnd, maxEnd;

        public override void TriggerTransition(bool forceReset)
        {
            if(state == TransitionState.AnyState)//this is for brand new items, it show we don't know what state it is in. However since we are calling fade out first...
                state = TransitionState.Finished;//then it must be finished and waiting to fade in

            if(!forceReset)
                if(state != BaseTransition.TransitionState.Finished && state != BaseTransition.TransitionState.Waiting)
                    return;

            differenceStartPoint = parent.transform.position;

            #region Start At Current
            if(startAtCurrent)//if the start point should be the current position
            {
                switch(type)
                {
                    case MovementType.Absolute:
                        startPoint = parent.transform.position;//then update it with the current position
                        break;

                    case MovementType.Local:
                        startPoint = parent.transform.localPosition;
                        break;
                }
            }
            #endregion

            if(deviateEnd)
                endPoint = Vector3.Lerp(minEnd, maxEnd, Random.value);

            base.TriggerTransition(forceReset);
        }

        public override void TriggerFadeOut()
        {
            if(endAtCurrent)//if we need to update the end point to ensure we start smoothly
                switch(type)
                {
                    case MovementType.Absolute:
                        endPoint = transform.position;//then update it with the current position
                        break;

                    case MovementType.Local:
                        endPoint = parent.transform.localPosition;
                        break;
                }

            if(deviateStart)
                startPoint = Vector3.Lerp(minStart, maxStart, Random.value);//if we should deviate the start value then update it

            base.TriggerFadeOut();
        }

        protected override void Transition(float transitionPercentage)
        {
            if(transform == null)
            {
                transform = parent.transform;

                if(transform == null)//if still null
                {
                    Stop();
                    return;//don't try to animate something with no transform, can happen when destroying things
                }
            }

            switch(type)
            {
                case MovementType.Absolute:
                    transform.position = startPoint + (endPoint - startPoint) * transitionPercentage;
                    break;

                #region Difference
                case MovementType.Difference:
                    Vector3 difference = endPoint - startPoint;

                    if(difference.x == 0)//if we are not affecting this axis
                        differenceStartPoint.x = parent.transform.position.x;//then update it from the current position

                    if(difference.y == 0)
                        differenceStartPoint.y = parent.transform.position.y;

                    if(difference.z == 0)
                        differenceStartPoint.z = parent.transform.position.z;

                    parent.transform.position = differenceStartPoint + difference * transitionPercentage;//the important difference here is that this ignores aniamtions with no value. E.G if x = 0 then X wont move
                    break;
                #endregion

                case MovementType.Local:
                    transform.localPosition = startPoint + (endPoint - startPoint) * transitionPercentage;
                    break;

                case MovementType.RectTransform:
                    (transform as RectTransform).anchoredPosition3D = startPoint + (endPoint - startPoint) * transitionPercentage;
                    break;
            }
        }

        public override void Clone(BaseTransition other)
        {
            base.Clone(other);

            MovingTransition converted = (MovingTransition)other;

            type = converted.type;
            differenceStartPoint = converted.differenceStartPoint;
            startPoint = converted.startPoint;
            endPoint = converted.endPoint;
        }

        public override void TriggerTransitionWithoutDelay()
        {
            base.TriggerTransitionWithoutDelay();

            differenceStartPoint = parent.transform.position;
        }

        public override void JumpToStart()
        {
            if(startAtCurrent)
            {
                if(type == MovementType.Absolute)
                    startPoint = parent.transform.position;
                else if(type == MovementType.Local)
                    startPoint = parent.transform.localPosition;
            }

            base.JumpToStart();
        }

        #region Editor Externals
#if(UNITY_EDITOR)
        /// <summary>
        /// This is mainly used by the editor when the type changes to update the start and end points accordingly
        /// </summary>
        public void SetType(MovementType newType)
        {
            MovementType previousType = type;
            Vector3 currentPosition = parent.transform.position;//store the old position

            type = previousType;
            ViewPosition(TransitionalObject.MovingDataType.StartPoint);//so view the position in the old type
            type = newType;//set to the new type
            UpdatePosition(TransitionalObject.MovingDataType.StartPoint);//and update position

            type = previousType;
            ViewPosition(TransitionalObject.MovingDataType.MaxStart);//so view the position in the old type
            type = newType;//set to the new type
            UpdatePosition(TransitionalObject.MovingDataType.MaxStart);//and update position

            type = previousType;
            ViewPosition(TransitionalObject.MovingDataType.EndPoint);//so view the position in the old type
            type = newType;//set to the new type
            UpdatePosition(TransitionalObject.MovingDataType.EndPoint);//and update position

            type = previousType;
            ViewPosition(TransitionalObject.MovingDataType.MaxEnd);//so view the position in the old type
            type = newType;//set to the new type
            UpdatePosition(TransitionalObject.MovingDataType.MaxEnd);//and update position

            parent.transform.position = currentPosition;//restore
        }

        /// <summary>
        /// Called by the editor to view either the start of end point
        /// </summary>
        public override void ViewPosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
            {
                if(type == MovementType.Local)
                    parent.transform.localPosition = startPoint;
                else if(type == MovementType.RectTransform)
                    (parent.transform as RectTransform).anchoredPosition3D = startPoint;
                else
                    parent.transform.position = startPoint;
            }
            else if(movingType == TransitionalObject.MovingDataType.MaxStart)
            {
                if(type == MovementType.Local)
                    parent.transform.localPosition = maxStart;
                else if(type == MovementType.RectTransform)
                    (parent.transform as RectTransform).anchoredPosition3D = maxStart;
                else
                    parent.transform.position = maxStart;
            }
            else if(movingType == TransitionalObject.MovingDataType.MaxEnd)
            {
                if(type == MovementType.Local)
                    parent.transform.localPosition = maxEnd;
                else if(type == MovementType.RectTransform)
                    (parent.transform as RectTransform).anchoredPosition3D = maxEnd;
                else
                    parent.transform.position = maxEnd;
            }
            else
            {
                if(type == MovementType.Local)
                    parent.transform.localPosition = endPoint;
                else if(type == MovementType.RectTransform)
                    (parent.transform as RectTransform).anchoredPosition3D = endPoint;
                else
                    parent.transform.position = endPoint;
            }
        }

        /// <summary>
        /// Called by the editor to update the start and end points based on the current position
        /// </summary>
        /// <param name="isStartPoint"></param>
        public override void UpdatePosition(TransitionalObject.MovingDataType movingType)
        {
            if(movingType == TransitionalObject.MovingDataType.StartPoint)
            {
                if(type == MovementType.Local)
                    startPoint = parent.transform.localPosition;
                else if(type == MovementType.RectTransform)
                    startPoint = (parent.transform as RectTransform).anchoredPosition3D;
                else
                    startPoint = parent.transform.position;

                minStart = startPoint;
            }
            else if(movingType == TransitionalObject.MovingDataType.MaxStart)
            {
                if(type == MovementType.Local)
                    maxStart = parent.transform.localPosition;
                else if(type == MovementType.RectTransform)
                    maxStart = (parent.transform as RectTransform).anchoredPosition3D;
                else
                    maxStart = parent.transform.position;
            }
            else if(movingType == TransitionalObject.MovingDataType.MaxEnd)
            {
                if(type == MovementType.Local)
                    maxEnd = parent.transform.localPosition;
                else if(type == MovementType.RectTransform)
                    maxEnd = (parent.transform as RectTransform).anchoredPosition3D;
                else
                    maxEnd = parent.transform.position;
            }
            else
            {
                if(type == MovementType.Local)
                    endPoint = parent.transform.localPosition;
                else if(type == MovementType.RectTransform)
                    endPoint = (parent.transform as RectTransform).anchoredPosition3D;
                else
                    endPoint = parent.transform.position;

                minEnd = endPoint;
            }
        }

        public override void SwapDataFields()
        {
            Vector3 temp = startPoint;

            startPoint = endPoint;
            endPoint = temp;
        }
#endif
        #endregion
    }
}