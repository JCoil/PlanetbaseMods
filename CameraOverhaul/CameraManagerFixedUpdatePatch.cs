using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;
using PlanetbaseModUtilities;
using static UnityModManagerNet.UnityModManager;
using UnityEngine;
using HarmonyLib;

namespace CameraOverhaul
{
    [HarmonyPatch(typeof(CameraManager), "fixedUpdate")]
    public static class CameraManagerFixedUpdatePatch
    {
        public static bool Prefix(ref CameraManager __instance, float timeStep, int frameIndex)
        {
            var cameraManager = __instance;

            if (!cameraManager.isCinematic())
            {
                float lateralMoveSpeed = timeStep * 6f;
                float zoomAndRotationSpeed = timeStep * 10f;

                GameState gameState = GameManager.getInstance().getGameState();

                // this only happens when placing a module and only if current height < 21
                var currentHeight = cameraManager.GetCurrentHeight();
                var targetHeight = cameraManager.GetTargetHeight();

                if (targetHeight != currentHeight)
                {
                    cameraManager.SetCurrentHeight(currentHeight + Mathf.Sign(targetHeight - currentHeight) * timeStep * 30f);

                    if (Mathf.Abs(currentHeight - targetHeight) < 0.5f)
                    {
                        cameraManager.SetCurrentHeight(targetHeight);
                    }
                }

                if (gameState is GameStateGame game && !game.isCameraFixed() && !TimeManager.getInstance().isPaused())
                {
                    KeyBindingManager keyBindingManager = KeyBindingManager.getInstance();

                    if (game != null && game.GetModeValue() == CameraOverhaul.ModePlacingModule)
                    {
                        if (!CameraOverhaul.mIsPlacingModule)
                        {
                            CameraOverhaul.mIsPlacingModule = true;
                            CameraOverhaul.mModulesize = game.GetCurrentModuleSize();
                        }

                        var mZoomAxis = cameraManager.GetZoomAxis();

                        // we're zooming
                        if (Mathf.Abs(mZoomAxis) > 0.001f || Mathf.Abs(keyBindingManager.getCompositeAxis(ActionType.CameraZoomOut, ActionType.CameraZoomIn)) > 0.001f)
                        {
                            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                            {
                                if (mZoomAxis <= -0.1f || keyBindingManager.getBinding(ActionType.CameraZoomOut).justUp())
                                {
                                    if (CameraOverhaul.mModulesize > game.GetPlacedModuleType().getMinSize())
                                    {
                                        CameraOverhaul.mModulesize--;
                                    }
                                }
                                else if ((mZoomAxis >= 0.1f || keyBindingManager.getBinding(ActionType.CameraZoomIn).justUp()) && CameraOverhaul.mModulesize < game.GetPlacedModuleType().getMaxSize())
                                {
                                    CameraOverhaul.mModulesize++;
                                }
                            }
                        }

                        game.SetCurrentModuleSize(CameraOverhaul.mModulesize);
                    }
                    else
                    {
                        CameraOverhaul.mIsPlacingModule = false;
                    }

                    var mAcceleration = cameraManager.GetAcceleration();

                    mAcceleration.x += keyBindingManager.getCompositeAxis(ActionType.CameraMoveLeft, ActionType.CameraMoveRight) * lateralMoveSpeed;
                    mAcceleration.z += keyBindingManager.getCompositeAxis(ActionType.CameraMoveBack, ActionType.CameraMoveForward) * lateralMoveSpeed;

                    if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                    {
                        mAcceleration.y -= cameraManager.GetZoomAxis() * zoomAndRotationSpeed;
                        mAcceleration.y -= keyBindingManager.getCompositeAxis(ActionType.CameraZoomOut, ActionType.CameraZoomIn) * zoomAndRotationSpeed;
                    }

                    CameraOverhaul.mAlternateRotationAcceleration -= keyBindingManager.getCompositeAxis(ActionType.CameraRotateLeft, ActionType.CameraRotateRight) * zoomAndRotationSpeed;

                    // Rotate with middle mouse button
                    if (Input.GetMouseButton(2))
                    {
                        float mouseDeltaX = Input.mousePosition.x - cameraManager.GetPreviousMouseX();
                        if (Mathf.Abs(mouseDeltaX) > Mathf.Epsilon)
                        {
                            cameraManager.SetRotationAcceleration(cameraManager.GetRotationAcceleration() + zoomAndRotationSpeed * mouseDeltaX * 0.1f);
                        }

                        float mouseDeltaY = Input.mousePosition.y - CameraOverhaul.mPreviousMouseY;
                        if (Mathf.Abs(mouseDeltaY) > Mathf.Epsilon)
                        {
                            CameraOverhaul.mVerticalRotationAcceleration += zoomAndRotationSpeed * mouseDeltaY * 0.1f;
                        }
                    }

                    // Move with mouse on screen borders
                    if (!Application.isEditor)
                    {
                        float screenBorder = Screen.height * 0.01f;
                        if (Input.mousePosition.x < screenBorder)
                        {
                            mAcceleration.x = mAcceleration.x - lateralMoveSpeed;
                        }
                        else if (Input.mousePosition.x > Screen.width - screenBorder)
                        {
                            mAcceleration.x = mAcceleration.x + lateralMoveSpeed;
                        }
                        if (Input.mousePosition.y < screenBorder)
                        {
                            mAcceleration.z = mAcceleration.z - lateralMoveSpeed;
                        }
                        else if (Input.mousePosition.y > Screen.height - screenBorder)
                        {
                            mAcceleration.z = mAcceleration.z + lateralMoveSpeed;
                        }
                    }

                    float clampSpeed = !Input.GetKey(KeyCode.LeftShift) ? 1f : 0.25f;
                    mAcceleration.x = Mathf.Clamp(mAcceleration.x - mAcceleration.x * lateralMoveSpeed, -clampSpeed, clampSpeed);
                    mAcceleration.z = Mathf.Clamp(mAcceleration.z - mAcceleration.z * lateralMoveSpeed, -clampSpeed, clampSpeed);
                    mAcceleration.y = Mathf.Clamp(mAcceleration.y - mAcceleration.y * zoomAndRotationSpeed, -clampSpeed, clampSpeed);

                    cameraManager.SetAcceleration(mAcceleration);
                    cameraManager.SetRotationAcceleration(Mathf.Clamp(cameraManager.GetRotationAcceleration() - cameraManager.GetRotationAcceleration() * zoomAndRotationSpeed, -clampSpeed, clampSpeed));

                    CameraOverhaul.mVerticalRotationAcceleration = Mathf.Clamp(CameraOverhaul.mVerticalRotationAcceleration - CameraOverhaul.mVerticalRotationAcceleration * zoomAndRotationSpeed, -clampSpeed, clampSpeed);
                    CameraOverhaul.mAlternateRotationAcceleration = Mathf.Clamp(CameraOverhaul.mAlternateRotationAcceleration - CameraOverhaul.mAlternateRotationAcceleration * zoomAndRotationSpeed, -clampSpeed, clampSpeed);
                }
                else
                {
                    cameraManager.SetAcceleration(Vector3.zero);
                    cameraManager.SetRotationAcceleration(0f);
                    CameraOverhaul.mVerticalRotationAcceleration = 0f;
                    CameraOverhaul.mAlternateRotationAcceleration = 0f;
                }

                cameraManager.SetPreviousMouseX(Input.mousePosition.x);
                CameraOverhaul.mPreviousMouseY = Input.mousePosition.y;
            }
            else
            {
                cameraManager.getCinematic().fixedUpdate(timeStep);
            }

            cameraManager.SetZoomAxis(0f);

            return false; // Want to skip original code
        }
    }
}
