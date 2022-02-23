using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;
using Planetbase;
using static UnityModManagerNet.UnityModManager;
using UnityEngine;
using HarmonyLib;

namespace CameraOverhaul
{
    public class CameraOverhaul : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new CameraOverhaul(), modEntry, "CameraOverhaul");

        public override void OnInitialized()
        {
            // Nothing needed here
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            // Nothing needed here
        }
    }

    [HarmonyPatch(typeof(CameraManager), "update")]
    public abstract class CameraManagerPatch
    {
        // These are not const so other mods can change them if they want
        public static float MIN_HEIGHT = 12f;
        public static float MAX_HEIGHT = 120f;

        private static float mVerticalRotationAcceleration = 0f;
        private static float mPreviousMouseY = 0f;

        private static float mAlternateRotationAcceleration = 0f;

        private static Plane mGroundPlane = new Plane(Vector3.up, new Vector3(TerrainGenerator.TotalSize, 0f, TerrainGenerator.TotalSize) * 0.5f);

        private static int mModulesize = 0;
        private static bool mIsPlacingModule = false;

        public static bool Prefix(ref CameraManager __instance, float timeStep)
        {
            var cameraManager = __instance;

            if (cameraManager.mZoomAxis == 0f)
                mZoomAxis = Input.GetAxis("Zoom");

            GameState gameState = GameManager.getInstance().getGameState();
            if (gameState != null && !gameState.isCameraFixed())
            {
                if (mCinematic == null)
                {
                    GameStateGame game = gameState as GameStateGame;
                    if (game != null && game.mMode == GameStateGame.Mode.PlacingModule && mIsPlacingModule)
                    {
                        game.mCurrentModuleSize = mModulesize;
                    }

                    Transform transform = mMainCamera.transform;

                    float xAxis = mAcceleration.x;
                    float yAxis = mAcceleration.y;
                    float zAxis = mAcceleration.z;
                    float absXAxis = Mathf.Abs(xAxis);
                    float absYAxis = Mathf.Abs(yAxis);
                    float absZAxis = Mathf.Abs(zAxis);

                    if (!mLocked)
                    {
                        // if zooming
                        if (absYAxis > 0.01f)
                        {
                            float speed = Mathf.Clamp(60f * timeStep, 0.01f, 100f);
                            float newHeight = Mathf.Clamp(mCurrentHeight + yAxis * speed, MIN_HEIGHT, MAX_HEIGHT);

                            if (transform.eulerAngles.x < 86f)
                            {
                                zAxis += (mCurrentHeight - newHeight) / speed;
                                absZAxis = Mathf.Abs(zAxis);
                            }

                            mCurrentHeight = newHeight;
                            mTargetHeight = mCurrentHeight;
                        }

                        // Move forwards
                        if (absZAxis > 0.001f)
                        {
                            transform.position += new Vector3(transform.forward.x, 0f, transform.forward.z).normalized * zAxis * timeStep * 80f;
                        }

                        // Move sideways
                        if (absXAxis > 0.001f)
                        {
                            transform.position += new Vector3(transform.right.x, 0f, transform.right.z).normalized * xAxis * timeStep * 80f;
                        }

                        // rotate around cam
                        Vector3 eulerAngles = transform.eulerAngles;
                        if (Mathf.Abs(mRotationAcceleration) > 0.01f)
                        {
                            eulerAngles.y += mRotationAcceleration * timeStep * 120f;
                        }

                        if (Mathf.Abs(mVerticalRotationAcceleration) > 0.01f)
                        {
                            eulerAngles.x = Mathf.Clamp(eulerAngles.x - mVerticalRotationAcceleration * timeStep * 120f, 20f, 87f);
                        }
                        transform.eulerAngles = eulerAngles;
                    }
                    else if (absYAxis > 0.01f)
                    {
                        float speed = Mathf.Clamp(60f * timeStep, 0.01f, 100f);
                        Vector3 movement = transform.forward * speed * -yAxis;

                        Vector3 planePoint = Selection.getSelectedConstruction().getPosition();
                        planePoint.y = yAxis < 0f ? 4f : Selection.getSelectedConstruction().getRadius() + 10f;
                        Plane plane = new Plane(Vector3.up, planePoint);

                        Ray ray = new Ray(transform.position, yAxis < 0f ? transform.forward : -transform.forward);
                        float dist;
                        if (plane.Raycast(ray, out dist))
                        {
                            if (dist < movement.magnitude)
                                movement *= dist / movement.magnitude;

                            transform.position += movement;
                        }
                    }

                    // rotate around world
                    if (Mathf.Abs(mAlternateRotationAcceleration) > 0.01f)
                    {
                        Ray ray = new Ray(transform.position, transform.forward);
                        float dist;
                        if (mGroundPlane.Raycast(ray, out dist))
                        {
                            transform.RotateAround(transform.position + transform.forward * dist, Vector3.up, mAlternateRotationAcceleration * timeStep * 120f);
                        }
                    }

                    // if we moved, set the correct height
                    if (!mLocked && (absZAxis > 0.001f || absXAxis > 0.001f || absYAxis > 0.01f))
                    {
                        placeOnFloor(mCurrentHeight);
                    }

                    // Calc map center and distance
                    Vector3 mapCenter = new Vector3(TerrainGenerator.TotalSize, 0f, TerrainGenerator.TotalSize) * 0.5f;
                    Vector3 mapCenterToCam = transform.position - mapCenter;
                    float distToMapCenter = mapCenterToCam.magnitude;

                    // limit cam to 375 units from center
                    if (distToMapCenter > 375f)
                    {
                        mMainCamera.transform.position = mapCenter + mapCenterToCam.normalized * 375f;
                    }
                }
                else
                {
                    updateCinematic(timeStep);
                }
            }

            // interpolate the position when the game moves the camera to a specific location (e.g when editing a building)
            if (mCameraTransition < 1f)
            {
                if (mTransitionTime == 0f)
                {
                    mCameraTransition = 1f;
                }
                else
                {
                    float num4 = timeStep / mTransitionTime;
                    mCameraTransition += num4;
                }
                mCurrentTransform.interpolate(mSourceTransform, mTargetTransform, mCameraTransition);
                mCurrentTransform.apply(mMainCamera.transform);
            }

            mSkydomeCamera.transform.rotation = mMainCamera.transform.rotation;
        }

        [RedirectFrom(typeof(CameraManager))]
        public new void fixedUpdate(float timeStep, int frameIndex)
        {
            if (this.mCinematic == null)
            {
                float lateralMoveSpeed = timeStep * 6f;
                float zoomAndRotationSpeed = timeStep * 10f;

                GameState gameState = GameManager.getInstance().getGameState();

                // this only happens when placing a module and only if current height < 21
                if (mTargetHeight != mCurrentHeight)
                {
                    mCurrentHeight += Mathf.Sign(mTargetHeight - mCurrentHeight) * timeStep * 30f;
                    if (Mathf.Abs(mCurrentHeight - mTargetHeight) < 0.5f)
                    {
                        mCurrentHeight = mTargetHeight;
                    }
                }


                if (gameState != null && !gameState.isCameraFixed() && !TimeManager.getInstance().isPaused())
                {
                    KeyBindingManager keyBindingManager = KeyBindingManager.getInstance();
                    GameStateGame game = gameState as GameStateGame;
                    if (game != null && game.mMode == GameStateGame.Mode.PlacingModule)
                    {
                        if (!mIsPlacingModule)
                        {
                            mIsPlacingModule = true;
                            mModulesize = game.mCurrentModuleSize;
                        }

                        // we're zooming
                        if (Mathf.Abs(mZoomAxis) > 0.001f || Mathf.Abs(keyBindingManager.getCompositeAxis(ActionType.CameraZoomOut, ActionType.CameraZoomIn)) > 0.001f)
                        {
                            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                            {
                                if (mZoomAxis <= -0.1f || keyBindingManager.getBinding(ActionType.CameraZoomOut).justUp())
                                {
                                    if (mModulesize > game.mPlacedModuleType.getMinSize())
                                    {
                                        mModulesize--;
                                    }
                                }
                                else if ((mZoomAxis >= 0.1f || keyBindingManager.getBinding(ActionType.CameraZoomIn).justUp()) && mModulesize < game.mPlacedModuleType.getMaxSize())
                                {
                                    mModulesize++;
                                }
                            }
                        }

                        game.mCurrentModuleSize = mModulesize;
                    }
                    else
                    {
                        mIsPlacingModule = false;
                    }

                    mAcceleration.x += keyBindingManager.getCompositeAxis(ActionType.CameraMoveLeft, ActionType.CameraMoveRight) * lateralMoveSpeed;
                    mAcceleration.z += keyBindingManager.getCompositeAxis(ActionType.CameraMoveBack, ActionType.CameraMoveForward) * lateralMoveSpeed;

                    if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                    {
                        mAcceleration.y -= mZoomAxis * zoomAndRotationSpeed;
                        mAcceleration.y -= keyBindingManager.getCompositeAxis(ActionType.CameraZoomOut, ActionType.CameraZoomIn) * zoomAndRotationSpeed;
                    }

                    mAlternateRotationAcceleration -= keyBindingManager.getCompositeAxis(ActionType.CameraRotateLeft, ActionType.CameraRotateRight) * zoomAndRotationSpeed;

                    // Rotate with middle mouse button
                    if (Input.GetMouseButton(2))
                    {
                        float mouseDeltaX = Input.mousePosition.x - mPreviousMouseX;
                        if (Mathf.Abs(mouseDeltaX) > Mathf.Epsilon)
                            mRotationAcceleration += zoomAndRotationSpeed * mouseDeltaX * 0.1f;

                        float mouseDeltaY = Input.mousePosition.y - mPreviousMouseY;
                        if (Mathf.Abs(mouseDeltaY) > Mathf.Epsilon)
                            mVerticalRotationAcceleration += zoomAndRotationSpeed * mouseDeltaY * 0.1f;
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
                    mRotationAcceleration = Mathf.Clamp(mRotationAcceleration - mRotationAcceleration * zoomAndRotationSpeed, -clampSpeed, clampSpeed);
                    mVerticalRotationAcceleration = Mathf.Clamp(mVerticalRotationAcceleration - mVerticalRotationAcceleration * zoomAndRotationSpeed, -clampSpeed, clampSpeed);
                    mAlternateRotationAcceleration = Mathf.Clamp(mAlternateRotationAcceleration - mAlternateRotationAcceleration * zoomAndRotationSpeed, -clampSpeed, clampSpeed);
                }
                else
                {
                    mAcceleration = Vector3.zero;
                    mRotationAcceleration = 0f;
                    mVerticalRotationAcceleration = 0f;
                    mAlternateRotationAcceleration = 0f;
                }

                mPreviousMouseX = Input.mousePosition.x;
                mPreviousMouseY = Input.mousePosition.y;
            }
            else
            {
                mCinematic.fixedUpdate(timeStep);
            }

            mZoomAxis = 0f;
        }
    }
}
