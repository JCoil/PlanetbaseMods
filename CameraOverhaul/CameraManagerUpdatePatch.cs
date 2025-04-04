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
    [HarmonyPatch(typeof(CameraManager), "update")]
    public abstract class CameraManagerUpdatePatch
    {
        public static bool Prefix(ref CameraManager __instance, float timeStep)
        {
            var cameraManager = __instance;

            if (cameraManager.GetZoomAxis() == 0f)
            {
                cameraManager.SetZoomAxis(Input.GetAxis("Zoom"));
            }

            if (GameManager.getInstance().getGameState() is GameStateGame game && !game.isCameraFixed())
            {
                if (!cameraManager.isCinematic())
                {
                    if (game.IsMode(GameStateUtils.Mode.PlacingModule) && CameraOverhaul.mIsPlacingModule)
                    {
                        game.SetCurrentModuleSize(CameraOverhaul.mModulesize);
                    }

                    Transform transform = cameraManager.getCamera().transform;
                    var acceleration = cameraManager.GetAcceleration();

                    float xAxis = acceleration.x;
                    float yAxis = acceleration.y;
                    float zAxis = acceleration.z;
                    float absXAxis = Mathf.Abs(xAxis);
                    float absYAxis = Mathf.Abs(yAxis);
                    float absZAxis = Mathf.Abs(zAxis);

                    if (!cameraManager.IsLocked())
                    {
                        // if zooming
                        if (absYAxis > 0.01f)
                        {
                            float currentHeight = cameraManager.GetCurrentHeight();

                            // If enabled, scale speed by 0.5 times the currentHeight to minHeight ratio
                            float speedEasingFactor = CameraOverhaul.settings.ZoomSpeedEasingEnabled ?
                                (float)Math.Sqrt(currentHeight / CameraOverhaul.settings.MinHeight) : 1f;

                            float speed = Mathf.Clamp(60f * timeStep, 0.01f, 100f) * speedEasingFactor * CameraOverhaul.settings.ZoomSpeedMultiplier;
                            float newHeight = Mathf.Clamp(currentHeight + yAxis * speed, CameraOverhaul.settings.MinHeight, CameraOverhaul.settings.MaxHeight);

                            if (transform.eulerAngles.x < 86f) // If camera is not pointing close to vertical
                            {
                                // If enabled, scale z movement on zoom by inverse of height
                                float zMotionEasingfactor = CameraOverhaul.settings.ZoomZMotionEasingEnabled ?
                                    (2f * CameraOverhaul.settings.MinHeight / (currentHeight + CameraOverhaul.settings.MinHeight)) : 1f;

                                zAxis += zMotionEasingfactor * (currentHeight - newHeight) / speed;
                                absZAxis = Mathf.Abs(zAxis);                                
                            }

                            cameraManager.SetCurrentHeight(newHeight);
                            cameraManager.SetTargetHeight(newHeight);
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
                        if (Mathf.Abs(cameraManager.GetRotationAcceleration()) > 0.01f)
                        {
                            eulerAngles.y += cameraManager.GetRotationAcceleration() * timeStep * 120f;
                        }

                        if (Mathf.Abs(CameraOverhaul.mVerticalRotationAcceleration) > 0.01f)
                        {
                            eulerAngles.x = Mathf.Clamp(eulerAngles.x - CameraOverhaul.mVerticalRotationAcceleration * timeStep * 120f, 20f, 87f);
                        }

                        transform.eulerAngles = eulerAngles;
                    }
                    else if (absYAxis > 0.01f && Selection.getSelectedConstruction() is Construction construction)
                    {
                        float speed = Mathf.Clamp(60f * timeStep, 0.01f, 100f);
                        Vector3 movement = transform.forward * speed * -yAxis;

                        Vector3 planePoint = construction.getPosition();
                        planePoint.y = yAxis < 0f ? 4f : construction.getRadius() + 10f;
                        Plane plane = new Plane(Vector3.up, planePoint);

                        Ray ray = new Ray(transform.position, yAxis < 0f ? transform.forward : -transform.forward);

                        if (plane.Raycast(ray, out float dist))
                        {
                            if (dist < movement.magnitude)
                            {
                                movement *= dist / movement.magnitude;
                            }

                            transform.position += movement;
                        }
                    }

                    // rotate around world
                    if (Mathf.Abs(CameraOverhaul.mAlternateRotationAcceleration) > 0.01f)
                    {
                        Ray ray = new Ray(transform.position, transform.forward);
                        float dist;
                        if (CameraOverhaul.mGroundPlane.Raycast(ray, out dist))
                        {
                            transform.RotateAround(transform.position + transform.forward * dist, Vector3.up, CameraOverhaul.mAlternateRotationAcceleration * timeStep * 120f);
                        }
                    }

                    // if we moved, set the correct height
                    if (!cameraManager.IsLocked() && (absZAxis > 0.001f || absXAxis > 0.001f || absYAxis > 0.01f))
                    {
                        cameraManager.placeOnFloor(cameraManager.GetCurrentHeight());
                    }

                    // Calc map center and distance
                    Vector3 mapCenter = new Vector3(CameraOverhaul.TerrainTotalSize, 0f, CameraOverhaul.TerrainTotalSize) * 0.5f;
                    Vector3 mapCenterToCam = transform.position - mapCenter;
                    float distToMapCenter = mapCenterToCam.magnitude;

                    // limit cam to 375 units from center
                    if (distToMapCenter > 375f)
                    {
                        cameraManager.getCamera().transform.position = mapCenter + mapCenterToCam.normalized * 375f;
                    }
                }
                else
                {
                    cameraManager.updateCinematic(timeStep);
                }
            }

            // interpolate the position when the game moves the camera to a specific location (e.g when editing a building)
            if (cameraManager.isTransitioning())
            {
                if (cameraManager.GetTransitionTime() == 0f)
                {
                    cameraManager.SetCameraTransition(1f);
                }
                else
                {
                    float num4 = timeStep / cameraManager.GetTransitionTime();
                    cameraManager.SetCameraTransition(cameraManager.GetCameraTransition() + num4);
                }
                cameraManager.GetCurrentTransform().interpolate(
                    cameraManager.GetSourceTransform(),
                    cameraManager.GetTargetTransform(),
                    cameraManager.GetCameraTransition());

                cameraManager.GetCurrentTransform().apply(cameraManager.getCamera().transform);
            }

            cameraManager.GetSkydomeCamera().transform.rotation = cameraManager.getCamera().transform.rotation;

            return false; // Want to skip original code
        }
    }
}
