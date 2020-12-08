using UnityEngine;
using System.Collections;
using TransitionalObjects;
using UnityEngine.EventSystems;

namespace LaireonFramework
{
    public class CarouselElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [HideInInspector]
        public new RectTransform transform;

        CharacterCarousel carousel;

        public TransitionalObject transition;

        float animationOffset;//this is used to create an intro animation with more delay than usual
        float currentAnimation;
        float animationDelay;//prevents animation until this has passed
        float previousPosition = -1000;

        bool flipped;
        int index;

        void Awake()
        {
            transform = base.transform as RectTransform;
            transition.SetState(BaseTransition.TransitionState.TransitionIn);//show this transition is waiting since we will control it manually
            transition.enabled = false;//prevent the transition from upadting as well since this will handle the updates
        }

        public virtual void Initialise(CharacterCarousel carousel, int index)
        {
            this.carousel = carousel;
            this.index = index;
        }

        public void SetIntroAnimation(float delay, float animationTime)
        {
            animationDelay = delay;
            currentAnimation = animationTime;
            animationOffset = animationTime;

            if(flipped)//if the screen was flipped
                Flip();//flip back
        }

        public virtual void UpdatePosition(float currentPosition, float finalWidth)
        {
            #region Intro Animation
            if(currentAnimation > 0 || animationDelay > 0)
            {
                if(animationDelay > 0)//if in the delay
                {
                    currentPosition = 0;//stay hidden until the delay passes
                    animationDelay -= Time.deltaTime;
                }
                else
                {
                    currentPosition = Mathf.Lerp(currentPosition, 0, currentAnimation / animationOffset);//animates the screen moving into its true place
                    currentAnimation -= Time.deltaTime;
                }
            }
            #endregion

            if(currentPosition == previousPosition)
                return;
            else
                previousPosition = currentPosition;

            if(currentPosition > 2)//if stacked on the right
            {
                if(!flipped)//this can be true when first creating the carousel
                    Flip();

                transition.SetToPercentage(0);//initialise
                transform.anchoredPosition3D = ((MovingTransition)transition.transitions[1]).startPoint + new Vector3(finalWidth * (currentPosition - 2), 0, 0);//stagger and stack items on the end
            }
            else if(currentPosition > 1)//if fading out
            {
                if(!flipped)
                    Flip();

                transition.SetToPercentage(1 - (currentPosition - 1));//invert the flow
            }
            else if(currentPosition > 0)//middle section
            {
                if(flipped)
                    Flip();

                transition.SetToPercentage(currentPosition);
            }
            else//stacked on the left
            {
                transition.SetToPercentage(0);//initialise
                transform.anchoredPosition3D = ((MovingTransition)transition.transitions[1]).startPoint + new Vector3(finalWidth * (currentPosition - 2), 0, 0);//stagger and stack items at the start
            }
        }

        /// <summary>
        /// Flips our start and end points as you hover over the middle mark
        /// </summary>
        void Flip()
        {
            MovingTransition current = (MovingTransition)transition.transitions[1];//transitions 1 is the moving one in our prefab
            current.startPoint = new Vector3(current.startPoint.x * -1, current.startPoint.y, current.startPoint.z);//make sure to invert the end point!

            RotatingTransition currentRotating = (RotatingTransition)transition.transitions[2];//transitions 2 is the rotating one in our prefab
            currentRotating.reverseNegativeRotations = !currentRotating.reverseNegativeRotations;

            currentRotating.startPoint = new Vector3(currentRotating.startPoint.x, 360 - currentRotating.startPoint.y, currentRotating.startPoint.z);//flip the rotation as well

            flipped = !flipped;
        }

        #region Drag Events
        public void OnBeginDrag(PointerEventData eventData)
        {
            carousel.OnDragStart();
        }

        public void OnDrag(PointerEventData data)
        {
            carousel.currentPosition += data.delta.x / transform.sizeDelta.x;//the Pointer data is in pixels, so divide by the size
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            carousel.OnDragEnd(eventData.delta.x / transform.sizeDelta.x);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            carousel.JumpToView(index);
        }
        #endregion
    }
}