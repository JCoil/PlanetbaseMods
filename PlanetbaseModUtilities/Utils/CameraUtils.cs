using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlanetbaseModUtilities
{
    public static class CameraUtils
    {
        #region Getters

        public static float GetZoomAxis(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, float>("mZoomAxis", cameraManager);
        }

        public static Vector3 GetAcceleration(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, Vector3>("mAcceleration", cameraManager);
        }

        public static float GetRotationAcceleration(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, float>("mRotationAcceleration", cameraManager);
        }

        public static float GetCurrentHeight(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, float>("mCurrentHeight", cameraManager);
        }

        public static float GetTargetHeight(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, float>("mTargetHeight", cameraManager);
        }

        public static bool IsLocked(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, bool>("mLocked", cameraManager);
        }

        public static float GetCameraTransition(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, float>("mCameraTransition", cameraManager);
        }

        public static float GetTransitionTime(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, float>("mTransitionTime", cameraManager);
        }

        public static SimpleTransform GetCurrentTransform(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, SimpleTransform>("mCurrentTransform", cameraManager);
        }

        public static SimpleTransform GetSourceTransform(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, SimpleTransform>("mSourceTransform", cameraManager);
        }

        public static SimpleTransform GetTargetTransform(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, SimpleTransform>("mTargetTransform", cameraManager);
        }

        public static GameObject GetSkydomeCamera(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, GameObject>("mSkydomeCamera", cameraManager);
        }

        public static float GetPreviousMouseX(this CameraManager cameraManager)
        {
            return CoreUtils.GetMember<CameraManager, float>("mPreviousMouseX", cameraManager);
        }

        #endregion

        #region Setters

        public static void SetZoomAxis(this CameraManager cameraManager, float zoomAxis)
        {
            CoreUtils.SetMember("mZoomAxis", cameraManager, zoomAxis);
        }

        public static void SetCurrentHeight(this CameraManager cameraManager, float height)
        {
            CoreUtils.SetMember("mCurrentHeight", cameraManager, height);
        }

        public static void SetTargetHeight(this CameraManager cameraManager, float height)
        {
            CoreUtils.SetMember("mTargetHeight", cameraManager, height);
        }

        public static void SetRotationAcceleration(this CameraManager cameraManager, float rotationAcceleration)
        {
            CoreUtils.SetMember("mRotationAcceleration", cameraManager, rotationAcceleration);
        }

        public static void SetAcceleration(this CameraManager cameraManager, Vector3 acceleration)
        {
            CoreUtils.SetMember("mAcceleration", cameraManager, acceleration);
        }

        public static void SetCameraTransition(this CameraManager cameraManager, float transition)
        {
            CoreUtils.SetMember("mCameraTransition", cameraManager, transition);
        }

        public static void SetPreviousMouseX(this CameraManager cameraManager, float mouseX)
        {
            CoreUtils.SetMember("mPreviousMouseX", cameraManager, mouseX);
        }

        #endregion

        #region Methods


        #endregion
    }
}
