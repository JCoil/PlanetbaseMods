using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using PlanetbaseModUtilities.Utils;
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

        public static void Postfix(ref Module ___mActiveModule, ref int ___mCurrentModuleSize)
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                if (SnapToClosestAvailableAlignmentPosition(___mActiveModule, Module.ValidSizes[___mCurrentModuleSize], ___mActiveModule.getPosition(), 
                    out Vector3 snappedPosition))
                {
                    IsRendering = true;
                    snappedPosition.y = TerrainGenerator.getInstance().getFloorHeight();

                    ___mActiveModule.setValidPosition(true);
                    ___mActiveModule.setPosition(snappedPosition);
                }
            }
            else
            {
                IsRendering = false;
                DebugRendererUtils.ClearGroup("Connections");
            }
        }     
      
        /// <summary>
        /// This method is around 10x faster than the original with essentially the same result. This has a noticeable performance impact on very large bases.
        /// We order our snap points by closest first to massively reduce the number of times we have to call canPlaceModule which is very expensive
        /// </summary>
        public static bool SnapToClosestAvailableAlignmentPosition(Module mActiveModule, float size, Vector3 referencePosition, out Vector3 snappedPosition)
        {
            snappedPosition = referencePosition;
            var connectionFound = false;

            var nearbyModules = BuildableUtils.GetAllModules().Where(x => x != null && x != mActiveModule && Vector3.Distance(x.getPosition(), referencePosition) < 50f);
            var orderedSnapPoints = nearbyModules.SelectMany(x => GetPositionsAroundModule(x)).OrderBy(y => Vector3.Distance(y.Position, referencePosition)).ToList();

            foreach(var snapPoint in orderedSnapPoints)
            {
                if(Connection.canLink(mActiveModule, snapPoint.ParentModule, snapPoint.Position, snapPoint.ParentModule.getPosition()))
                {
                    if (!connectionFound && mActiveModule.canPlaceModule(snapPoint.Position, Vector3.up, size)) // We've already ordered our list so the first valid position is the closest
                    {
                        snappedPosition = snapPoint.Position;
                        connectionFound = true;
                    }

                    if (snapPoint.Index == 4) // It's the end point of a snap line
                    {
                        var lineEnd = snapPoint.Position + new Vector3(0, 0.1f, 0);
                        var lineStart = 
                            snapPoint.ParentModule.getPosition() 
                            + (snapPoint.Position - snapPoint.ParentModule.getPosition()).normalized * snapPoint.ParentModule.getRadius() // Edge of the module
                            + new Vector3(0, 0.1f, 0);
                        DebugRendererUtils.AddLine("Connections", lineStart, lineEnd, Color.blue, 0.5f);
                        DebugRendererUtils.AddCube("Connections", lineEnd, Color.blue, 1.0f);
                    }
                }
            }

            return connectionFound;
        }

        public static List<SnapPoint> GetPositionsAroundModule(Module module)
        {
            List<SnapPoint> positions = new List<SnapPoint>();

            Vector3 parentPosition = module.getPosition();
            Vector3 dir = module.getTransform().forward;
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var pos = parentPosition + dir * (10f + 5f * j);
                    positions.Add(new SnapPoint(j, pos, module));
                }

                dir = Quaternion.Euler(0f, 30f, 0f) * dir;
            }

            return positions;
        }
    }

    /// <summary>
    /// Just a handy struct to contain the relevant data
    /// </summary>
    public struct SnapPoint
    {
        public int Index;
        public Vector3 Position;
        public Module ParentModule;

        public SnapPoint(int index, Vector3 position, Module parent)
        {
            Index = index;
            Position = position;
            ParentModule = parent;
        }
    }
}
