using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class SetToZero : MonoBehaviour
    {
        #region Methods
        void Start()
        {
            transform.localPosition = Vector3.zero;
        }
        #endregion
    }
}
