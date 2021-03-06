﻿using UnityEngine;
using UnityEngine.Assertions;
using ITS.SharedUtility;

namespace ITS.GodHand
{
    public class GodHand : MonoBehaviour
    {
        private Rigidbody _Body;

        void Awake()
        {
            // DEBUG
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"GOD HAND - {this.name} - Awake()");
            #endif
            
            _Body = GetComponent<Rigidbody>();
            Assert.IsNotNull(_Body, "Rigid body not found for " + this.name + " GameObject!");
        }

        void OnMouseDrag()
        {
            // Not working, if you dont release the click, as soon as you go out of ui element, the drag is resumed
            if (!Utility.IsPointerOverUIObject())
                _Body.AddTorque(new Vector3(0.5f * Input.GetAxis("Mouse Y"), -0.5f * Input.GetAxis("Mouse X"), 0), ForceMode.Impulse);
        }
    }
}