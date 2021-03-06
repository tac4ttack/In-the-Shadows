﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.PostProcessing;
using ITS.GameManagement;

namespace ITS.PostProcessManagement
{
    public class PostProcessManager : MonoBehaviour
    {
        private PostProcessLayer _Cam_PostProcessLayer;

        public void InitCamAntialiasing()
        {
            _Cam_PostProcessLayer = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PostProcessLayer>();
            Assert.IsNotNull(_Cam_PostProcessLayer, "Post processing layer not found on main camera!");

            if (_Cam_PostProcessLayer == null)
            {
                return;
            }
            if (GameManager.GM.Settings.FXAAEnabled)
            {
                _Cam_PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
            }
            else
            {
                _Cam_PostProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
            }
        }
    }
}