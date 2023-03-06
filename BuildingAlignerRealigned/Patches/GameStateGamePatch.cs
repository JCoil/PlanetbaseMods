using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace BuildingAligner.Patches
{
    [HarmonyPatch(typeof(GameStateGame), "tryPlaceModule")]
    public class TryPlaceModulePatch
    {
        public static bool IsRendering = false;

        /// <summary>
        /// Hamrony transpiler is too complicated for me, so we'll just replicate the method and insert our code in the middle
        /// </summary>
        public static void Prefix(GameStateGame __instance,
            ref Module mActiveModule,
            ref int mCurrentModuleSize,
            ref ModuleType mPlacedModuleType,
            ref ResourceAmounts mCost,
            ref bool mRenderTops)
        {
            ClearDebugRendererConnections();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 256;
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 150f, layerMask))
            {
                float size = Module.ValidSizes[mCurrentModuleSize];
                if (mActiveModule == null)
                {
                    mActiveModule = Module.create(raycastHit.point, mCurrentModuleSize, mPlacedModuleType);
                    mActiveModule.setRenderTop(mRenderTops);
                    mActiveModule.setValidPosition(false);
                    mCost = mActiveModule.calculateCost();
                }

                if (mCurrentModuleSize != mActiveModule.getSizeIndex())
                {
                    mActiveModule.changeSize(mCurrentModuleSize);
                    mCost = mActiveModule.calculateCost();
                    mActiveModule.setValidPosition(false);
                }

                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {

                    raycastHit.point = (__instance as CustomGameStateGame).RenderAvailablePositions(__instance, mActiveModule, raycastHit.point);
                    //TryAlign(ref raycastHit);
                }
                else
                {
                    IsRendering = false;
                    ClearDebugRendererConnections();
                }

                bool flag = mActiveModule.canPlaceModule(raycastHit.point, raycastHit.normal, size);
                if (__instance.inTutorial())
                {
                    CoreUtils.InvokeMethod("snapToTutorialPosition", __instance, raycastHit, flag);
                }

                Vector3 point = raycastHit.point;
                float floorHeight = TerrainGenerator.getInstance().getFloorHeight();
                point.y = floorHeight;
                if (!mActiveModule.isValidPosition() || flag || (point - mActiveModule.getPosition()).magnitude > 5f)
                {
                    mActiveModule.setValidPosition(flag);
                    mActiveModule.setPosition(point);
                }
                mActiveModule.setPositionY(floorHeight + 0.1f);
            }
        }

        public static void ClearDebugRendererConnections()
        {
            //DebugRenderer.clearGroup("Connections");
        }
    }

    public abstract class CustomGameStateGame :GameStateGame
    {
        private CustomGameStateGame(string s, int i, Challenge c) : base(s, i, c) { }

        public  Vector3 RenderAvailablePositions(Module mActiveModule, Vector3 point)
        {
            float closestDist = float.MaxValue;
            Vector3 closestPos = point;
            float floorHeight = TerrainGenerator.getInstance().getFloorHeight();

            foreach (Module module in BuildableUtils.GetAllModules())
            {
                if (module == null || module == mActiveModule)
                    continue;

                Vector3 modulePos = module.getPosition();

                bool connectionAvailable = false;

                int count = 0;
                List<Vector3> positions = GetPositionsAroundModule(module);
                foreach (Vector3 position in positions)
                {
                    Vector3 p = position;
                    p.y = floorHeight;

                    float dist = Vector3.Distance(p, point);
                    if (dist < 35f && Connection.canLink(mActiveModule, module, p, modulePos) && CanPlaceModule(p, Vector3.up, Module.ValidSizes[mCurrentModuleSize]))
                    {
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestPos = p;
                        }

                        connectionAvailable = true;
                    }

                    if (count == 4 && connectionAvailable)
                    {
                        DebugRenderer.addLine("Connections", modulePos + (p - modulePos).normalized * module.getRadius(), p, Color.blue, 0.5f);
                        DebugRenderer.addCube("Connections", p, Color.blue, 1.0f);
                        connectionAvailable = false;
                    }

                    count = ++count % 5;
                }
            }

            //hit.point = closestPos;
            TryPlaceModulePatch.IsRendering = true;
            return closestPos;
        }

        public bool CanPlaceModule(Vector3 position, Vector3 normal, float radius)
        {
            float floorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();
            float heightDiff = position.y - floorHeight;

            bool isMine = mActiveModule.hasFlag(ModuleType.FlagMine);
            if (isMine)
            {
                if (heightDiff < 1f || heightDiff > 3f)
                {
                    // mine must be a little elevated
                    return false;
                }
            }
            else if (heightDiff > 0.1f || heightDiff < -0.1f)
            {
                // not at floor level
                return false;
            }

            // here we're approximating the circumference of the structure with 8 points
            // and will check that all these points are level with the floor
            float reducedRadius = radius * 0.75f;
            float angledReducedRadius = reducedRadius * 1.41421354f * 0.5f;
            Vector3[] circumference = new Vector3[]
            {
                position + new Vector3(reducedRadius, 0f, 0f),
                position + new Vector3(-reducedRadius, 0f, 0f),
                position + new Vector3(0f, 0f, reducedRadius),
                position + new Vector3(0f, 0f, -reducedRadius),
                position + new Vector3(angledReducedRadius, 0f, angledReducedRadius),
                position + new Vector3(angledReducedRadius, 0f, -angledReducedRadius),
                position + new Vector3(-angledReducedRadius, 0f, angledReducedRadius),
                position + new Vector3(-angledReducedRadius, 0f, -angledReducedRadius)
            };

            if (isMine)
            {
                // above we verified that it is a bit elevated
                // now make sure that at least one point is near level ground
                bool isValid = false;
                for (int i = 0; i < circumference.Length; i++)
                {
                    Vector3 floor;
                    PhysicsUtil.findFloor(circumference[i], out floor, 256);
                    if (floor.y < floorHeight + 1f || floor.y > floorHeight - 1f)
                    {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid)
                {
                    return false;
                }
            }
            else
            {
                // Make sure all points are near level ground
                for (int j = 0; j < circumference.Length; j++)
                {
                    Vector3 floor;
                    PhysicsUtil.findFloor(circumference[j], out floor, 256);
                    if (floor.y > floorHeight + 2f || floor.y < floorHeight - 1f)
                    {
                        return false;
                    }
                }
            }

            //position.y = floorHeight;

            // Can only be 375 units away from center of map
            Vector2 mapCenter = new Vector2(TerrainGenerator.TotalSize, TerrainGenerator.TotalSize) * 0.5f;
            float distToCenter = (mapCenter - new Vector2(position.x, position.z)).magnitude;
            if (distToCenter > 375f)
            {
                return false;
            }

            // anyPotentialLinks limits connection to 20 (on top of some other less relevant checks)
            //if (Module.mConstructions.Count > 1 && !anyPotentialLinks(position))
            //{
            //    return false;
            //}

            RaycastHit[] array2 = Physics.SphereCastAll(position + Vector3.up * 20f, radius * 0.5f + 3f, Vector3.down, 40f, 4198400);
            if (array2 != null)
            {
                for (int k = 0; k < array2.Length; k++)
                {
                    RaycastHit raycastHit = array2[k];
                    GameObject gameObject = raycastHit.collider.gameObject.transform.root.gameObject;
                    Construction construction = Construction.mConstructionDictionary[gameObject];
                    if (construction != null)
                    {
                        //if (construction is Connection)
                        //{
                        //    return false;
                        //}
                        float distToConstruction = (position - construction.getPosition()).magnitude - mActiveModule.getRadius() - construction.getRadius();
                        if (distToConstruction < 3f)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Not hitting construction: " + gameObject.name);
                    }
                }
            }

            // Check that it's away from the ship
            if (Physics.CheckSphere(position, radius * 0.5f + 3f, 65536))
            {
                return false;
            }

            // Check that it doesn't overlap materials
            //if (Physics.CheckSphere(position, radius * 0.5f + 2f, 1024))
            //{
            //    return false;
            //}

            // This is to rotate the mine. We're setting the mine as auto-rotate instead
            //if (isMine)
            //{
            //    Vector3 vector3 = new Vector3(normal.x, 0f, normal.z);
            //    Vector3 normalized = vector3.normalized;
            //    if (Vector3.Dot(this.mObject.transform.forward, normalized) < 0.8660254f)
            //    {
            //        this.mObject.transform.forward = normalized;
            //    }
            //}

            return true;
        }

        public static List<Vector3> GetPositionsAroundModule(Module module)
        {
            List<Vector3> positions = new List<Vector3>();

            Vector3 pos = module.getPosition();
            Vector3 dir = module.getTransform().forward;
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    positions.Add(pos + dir * (10f + 5f * j));
                }

                dir = Quaternion.Euler(0f, 30f, 0f) * dir;
            }

            return positions;
        }
    }
}
