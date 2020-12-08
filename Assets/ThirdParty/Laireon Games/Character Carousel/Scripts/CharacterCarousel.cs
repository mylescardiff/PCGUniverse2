using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class CharacterCarousel : MonoBehaviour
    {
        #region Variables
        [HideInInspector]
        public new Transform transform;

        List<CarouselElement> elementList = new List<CarouselElement>();

        public CarouselElement prefab;

        public int screensToSpawn = 10;//ten is just a test number

        [Tooltip("This is the animation time per element")]
        public float introAnimationTime = 0.25f;

        public float introDelay;

        /// <summary>
        /// The currently selected screen object
        /// </summary>
        public CarouselElement currentScreen
        {
            get
            {
                return elementList[currentScreenIndex];
            }
        }

        /// <summary>
        /// The index of the currently selected screen
        /// </summary>
        public int currentScreenIndex
        {
            get
            {
                return (int)((currentPosition - 1) / offsetBetweenElements);
            }
        }

        public float currentPosition;//used to scroll left and right

        [Tooltip("How much space to place between elements. This is relative to the elements width")]
        public float offsetBetweenElements = 0.5f;

        [Tooltip("This helps offset the ends of the animation to look like book pages")]
        public float edgeWidth = 25;

        [Tooltip("This allows for inertia. I.E as you swipe quickly it takes a second or so to come to a stop")]
        public float brakingTime = 0.5f;


        [Tooltip("The speed to move towards screens clicked directly")]
        public float jumpingTime = 0.2f;

        [Tooltip("How much time to spend accelerating")]
        public float jumpingAccelerationTime = 0.05f;

        [Tooltip(" How long it takes to snap into place whilst dragging the mouse")]
        public float snapTime;
        float currentSnapTime, startingSnap, targetSnap;

        float lastDelta, startingDelta;
        float currentTime;
        float jumpingRatio, targetJumpingRatio, currentJumpingAccelerationTime;//this helps to jump faster if you click 2 screens away rather than just the next one. The target just helps with acceleration
        bool snapped = true, ignoreClicks;
        float jumpIndex = -1;//which view, if any, to jump to
        #endregion

        #region Methods
        void Start()
        {
            Initialise();
        }

        public void Initialise()
        {
            transform = base.transform;

            if(screensToSpawn < 0)
            {
                enabled = false;
                return;
            }

            for(int i = 0; i < screensToSpawn; i++)
            {
                GameObject temp = Instantiate<GameObject>(prefab.gameObject);
                temp.transform.SetParent(transform);
                temp.transform.localScale = Vector3.one;
                temp.name = "Element " + i;
                elementList.Add(temp.GetComponent<CarouselElement>());
                elementList[i].Initialise(this, i);
            }

            //JumpToView((screensToSpawn + 1) / 2);//start by viewing the middle element
        }

        /// <summary>
        /// The delay will wait this time before starting the intro animation
        /// </summary>
        /// <param name="delay"></param>
        public void Reset(float delay)
        {
            if(delay > 0)
            {
                currentPosition = 1 + ((screensToSpawn + 1) / 2) * offsetBetweenElements;//jump instantly to the middle

                for(int i = 0; i < elementList.Count; i++)
                    elementList[i].SetIntroAnimation(delay + introDelay * i, introAnimationTime);//animate the intro
            }
            else
                JumpToView((screensToSpawn + 1) / 2);//start by viewing the middle element

            for(int i = 0; i < elementList.Count; i++)
                elementList[i].UpdatePosition(0, edgeWidth);//move all screens to the left
        }


        void Update()
        {
            #region Jumping to Screen
            if(jumpIndex > -1)//if jumping to an object
            {
                if(jumpingRatio < targetJumpingRatio)
                {
                    currentJumpingAccelerationTime += Time.deltaTime / jumpingAccelerationTime;//acceleration time, normalised to be between 0 and 1

                    jumpingRatio = targetJumpingRatio * currentJumpingAccelerationTime;//apply the acceleration
                }

                bool movingForward;//helps to prevent very quick snap backs by instantly snapping when we go over or under the value we want

                if(jumpIndex > currentPosition)
                {
                    currentPosition += offsetBetweenElements * (Time.deltaTime / jumpingTime) * jumpingRatio;
                    movingForward = true;
                }
                else
                {
                    currentPosition -= offsetBetweenElements * (Time.deltaTime / jumpingTime) * jumpingRatio;
                    movingForward = false;
                }

                if((movingForward && currentPosition > jumpIndex) || (!movingForward && currentPosition < jumpIndex))//if close to the view we need to snap to
                {
                    currentPosition = jumpIndex;
                    jumpIndex = -1;//stop jumping
                }
            }
            #endregion

            #region Snapping
            else if(!snapped)//if snapping or jumping to view an element             
            {
                currentSnapTime += Time.deltaTime / snapTime;

                currentPosition = Mathf.Lerp(startingSnap, targetSnap, currentSnapTime);

                if(currentSnapTime > 1)
                {
                    currentPosition = targetSnap;
                    snapped = true;
                    lastDelta = 0;
                }
            }
            #endregion

            #region Scrolling
            else if(lastDelta > 0.001 || lastDelta < -0.001)//if we just finished a swipe, then apply momentum
            {
                currentTime += Time.deltaTime / brakingTime;

                currentPosition += lastDelta;//keep moving with momentum. This is purposely ignoring deltaTime!

                lastDelta = startingDelta * (1 - currentTime);//slow the momentum as more time passes. This includes the delta

                if(currentTime > 1)
                    lastDelta = 0;//hard stop
            }
            #endregion

            #region Max and Min Caps
            if(currentPosition > 1 + ((screensToSpawn - 1) * offsetBetweenElements))//if on the last screen
                currentPosition = 1 + ((screensToSpawn - 1) * offsetBetweenElements);//cap at max
            else if(currentPosition < 1)
                currentPosition = 1;//cap at the min
            #endregion

            List<int> elementIndexes = new List<int>();

            for(int i = 0; i < elementList.Count; i++)
                if(elementList[i] != null)
                {
                    elementList[i].UpdatePosition(currentPosition - i * offsetBetweenElements, edgeWidth);

                    #region Depth Sorting
                    float current = currentPosition - i * offsetBetweenElements;

                    if(current >= 0.99)//if going off to the right
                        elementIndexes.Add(i);//then we want a low draw order priority by adding to the top
                    else
                        elementIndexes.Insert(0, i);//if we are on the left, then add to the front of the queue
                    #endregion
                }

            #region Sort for Drawing Order
            for(int i = 0; i < elementIndexes.Count; i++)
                elementList[elementIndexes[i]].transform.SetSiblingIndex(i);//this ensures the middle element, the one with the smallest z, will always draw first
            #endregion

            ProcessUserInput();//remove this if you dont want to jump screens according to keyboard input
        }

        void ProcessUserInput()
        {
            if(Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
                JumpToView(0);
            else if(Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
                JumpToView(1);
            else if(Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
                JumpToView(2);
            else if(Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
                JumpToView(3);
            else if(Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
                JumpToView(4);
            else if(Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
                JumpToView(5);
            else if(Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))
                JumpToView(6);
            else if(Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8))
                JumpToView(7);
            else if(Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9))
                JumpToView(8);

            else if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ignoreClicks = false;//always jump
                JumpToView(Mathf.CeilToInt((currentPosition - offsetBetweenElements) / offsetBetweenElements));//we round down here with ceil to prevent waiting for a snap before jumping again
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                ignoreClicks = false;//always jump
                JumpToView((int)((currentPosition - offsetBetweenElements) / offsetBetweenElements) - 2);
            }
        }

        public void OnDragStart()
        {
            jumpIndex = -1;//stop any jumps for other user input
            lastDelta = 0;
            ignoreClicks = true;
        }

        public void OnDragEnd(float delta)
        {
            ignoreClicks = false;
            startingDelta = delta;
            lastDelta = delta;
            currentTime = 0;
            snapped = false;
            jumpIndex = -1;

            float difference = (currentPosition - 1) % offsetBetweenElements;//how far we have left to move to snap into place

            if(difference < offsetBetweenElements / 2)//if we need to snap back
                targetSnap = currentPosition - difference;
            else
                targetSnap = currentPosition + (offsetBetweenElements - difference);

            currentSnapTime = 0;
            startingSnap = currentPosition;
        }

        /// <summary>
        /// Called whenever you click on a card, jumps the carousel to viewing it
        /// </summary>
        /// <param name="index"></param>
        public void JumpToView(int index)
        {
            if(!ignoreClicks)
            {
                jumpIndex = 1 + (index * offsetBetweenElements);
                currentTime = 0;
                jumpingRatio = 0;
                currentJumpingAccelerationTime = 0;
                targetJumpingRatio = Mathf.Abs(currentPosition - jumpIndex) / offsetBetweenElements;//basically how many screens are in the way to see the one they one, and thus how fast to move between screens
            }
        }
        #endregion
    }
}