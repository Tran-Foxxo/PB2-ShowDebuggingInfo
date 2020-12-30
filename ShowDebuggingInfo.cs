using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Common.Class;
using HarmonyLib;
using HarmonyLib.Tools;
using PolyPhysics;
using PolyPhysics.GameplayUtils;
using PolyPhysics.Viewers;
using PolyTechFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ShowDebuggingInfo
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(PolyTechFramework.PolyTechMain.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Poly Bridge 2.exe")]
    public class PluginMain : PolyTechMod
    {
        public const String PluginGuid = "polytech.hiddendebug";
        public const String PluginName = "Show Debugging Info";
        public const String PluginVersion = "0.5.0";

        private static BepInEx.Logging.ManualLogSource Staticlogger;

        private static bool IsEnabled
        {
            get
            {
                return Enabled.Value && PolyTechFramework.PolyTechMain.modEnabled.Value;
            }
        }
        
        public static ConfigEntry<bool> Enabled;
        //Physics Viewers
        public static ConfigEntry<bool> shapeViewerEnabled;
        public static ConfigEntry<bool> contactPointViewerEnabled;
        public static ConfigEntry<bool> aabbViewerEnabled;
        public static ConfigEntry<bool> inertiaViewerEnabled;
        public static ConfigEntry<bool> centerOfMassViewerEnabled;
        public static ConfigEntry<bool> nodeViewerEnabled;
        public static ConfigEntry<bool> edgeViewerEnabled;
        public static ConfigEntry<bool> edgeVelocityViewerEnabled;
        public static ConfigEntry<bool> nodeMassViewerEnabled;
        //World Debug
        public static ConfigEntry<bool> worldShowNodesEnabled;
        public static ConfigEntry<bool> worldShowEdgesEnabled;
        public static ConfigEntry<bool> worldShowEdgeIndicesEnabled;
        public static ConfigEntry<bool> worldShowNodeIndicesEnabled;
        public static ConfigEntry<bool> worldShowStressNumbersEnabled;
        //Other
        public static ConfigEntry<bool> wheelJointDebugEnabled;
        public static ConfigEntry<bool> m_DebugVisualizePolygonShapesForVehiclesEnabled;
        public static ConfigEntry<bool> NodePlacementUtilEnabled;
        //Unused features

        void Awake()
        {
            var harmony = new Harmony(PluginGuid);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Enabled = Config.Bind("͔Enable", "Enabled", true, new ConfigDescription("Turn this off to stop the mod", null, new ConfigurationManagerAttributes { Order = 9 }));
            //Physics Viewers
            string header = "͔Physics Viewers";
            shapeViewerEnabled = Config.Bind(header, "Shape Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            contactPointViewerEnabled = Config.Bind(header, "Contact Point Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            aabbViewerEnabled = Config.Bind(header, "Aabb Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            inertiaViewerEnabled = Config.Bind(header, "Inertia Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            centerOfMassViewerEnabled = Config.Bind(header, "Center Of Mass Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            nodeViewerEnabled = Config.Bind(header, "Node Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            edgeViewerEnabled = Config.Bind(header, "Edge Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            edgeVelocityViewerEnabled = Config.Bind(header, "Edge Velocity Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            nodeMassViewerEnabled = Config.Bind(header, "Node Mass Viewer", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            //World Debug
            header = "͔World (Needs level restart)";
            worldShowNodesEnabled = Config.Bind(header, "Show Nodes", true, new ConfigDescription("Used in multiple places", null, new ConfigurationManagerAttributes { Order = 5 }));
            worldShowEdgesEnabled = Config.Bind(header, "Show Edges", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            worldShowEdgeIndicesEnabled = Config.Bind(header, "Show Edge Indices", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            worldShowNodeIndicesEnabled = Config.Bind(header, "Show Node Indices", true, new ConfigDescription("Mostly unused but has references", null, new ConfigurationManagerAttributes { Order = 5 }));
            worldShowStressNumbersEnabled = Config.Bind(header, "Show Stress Numbers", true, new ConfigDescription("Mostly unused but has references", null, new ConfigurationManagerAttributes { Order = 5 }));
            //Other Debugging
            header = "Other";
            wheelJointDebugEnabled = Config.Bind(header, "Wheel Joint Debug", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));
            m_DebugVisualizePolygonShapesForVehiclesEnabled = Config.Bind(header, "Visualize Polygon Shapes For Vehicles", true, new ConfigDescription("Bridge.m_DebugVisualizePolygonShapesForVehicles", null, new ConfigurationManagerAttributes { Order = 5 }));
            NodePlacementUtilEnabled = Config.Bind(header, "Enable Node Placement Utils", true, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 5 }));

            //Unused features
            header = "Unused features";
            //TODO: Add any of these if they exist

            Config.SettingChanged += (o, e) =>
            {
                UpdateComponents();
            };

            PolyTechMain.registerMod(this);

        }
        //void Update()
        //{
        //}
        static void UpdateComponents()
        {
            Bridge.m_DebugVisualizePolygonShapesForVehicles = m_DebugVisualizePolygonShapesForVehiclesEnabled.Value && IsEnabled;

            GameObject physicsViewers = GameObject.Find("/Main/PolyPhysics/Physics Viewers/");
            physicsViewers.GetComponent<ShapeViewer>().enabled = shapeViewerEnabled.Value && IsEnabled;
            physicsViewers.GetComponent<ContactPointViewer>().enabled = contactPointViewerEnabled.Value && IsEnabled;
            physicsViewers.GetComponent<AabbViewer>().enabled = aabbViewerEnabled.Value && IsEnabled;
            physicsViewers.GetComponent<InertiaViewer>().enabled = inertiaViewerEnabled.Value && IsEnabled;
            physicsViewers.GetComponent<CenterOfMassViewer>().enabled = centerOfMassViewerEnabled.Value && IsEnabled;
            physicsViewers.GetComponent<NodeViewer>().enabled = nodeViewerEnabled.Value && IsEnabled;
            physicsViewers.GetComponent<EdgeViewer>().enabled = edgeViewerEnabled.Value && IsEnabled;
            physicsViewers.GetComponent<EdgeVelocityViewer>().enabled = edgeVelocityViewerEnabled.Value && IsEnabled;
            physicsViewers.GetComponent<NodeMassViewer>().enabled = nodeMassViewerEnabled.Value && IsEnabled;

            World world = GetPolyPhysicsWorld();
            world.showNodes = worldShowNodesEnabled.Value && IsEnabled;
            world.showEdges = worldShowEdgesEnabled.Value && IsEnabled;
            world.showEdgeIndices = worldShowEdgeIndicesEnabled.Value && IsEnabled;
            world.showNodeIndices = worldShowNodeIndicesEnabled.Value && IsEnabled;
            world.showStressNumbers = worldShowStressNumbersEnabled.Value && IsEnabled;

            GameObject buildtimeViewers = GameObject.Find("/Main/PolyPhysics/Build-Time Viewers/");
            buildtimeViewers.GetComponent<NodePlacementUtil>().enabled = NodePlacementUtilEnabled.Value && IsEnabled;
        }
        public static World GetPolyPhysicsWorld()
        {
            GameObject obj = GameObject.Find("/Main/PolyPhysics/World, Hydraulics, Listeners/");
            return obj.GetComponent<World>();
        }
        public static void OnDrawGizmos(object obj)
        {
            Traverse.Create(obj).Method("OnDrawGizmos").GetValue();
        }
        //==================== INIT STUFF ====================
        [HarmonyPatch(typeof(GameManager), "LoadFirstLevel")]
        public static class init
        {
            [HarmonyPostfix]
            static void Postfix()
            {
                UpdateComponents();
            }
        }
        //==================== WORLD PATCHES ====================
        [HarmonyPatch(typeof(PolyPhysics.World), "FixedUpdate")]
        public static class patchBridgeUpdate
        {
            [HarmonyPostfix]
            static void Postfix(ref PolyPhysics.World __instance)
            {
                if (IsEnabled)
                {
                    OnDrawGizmos(__instance);
                }
            }
        }
        //==================== WHEELJOINT PATCHES ====================
        [HarmonyPatch(typeof(PolyPhysics.WheelJoint), "Awake")]
        public static class patchWheelJoint
        {
            [HarmonyPostfix]
            static void Postfix(ref PolyPhysics.WheelJoint __instance)
            {
                __instance.drawGizmos = IsEnabled && wheelJointDebugEnabled.Value;
            }
        }
        [HarmonyPatch(typeof(PolyPhysics.WheelJoint), "UpdateState")]
        public static class patchWheelJointUpdate
        {
            [HarmonyPostfix]
            static void Postfix(ref PolyPhysics.WheelJoint __instance)
            {
                if (IsEnabled && wheelJointDebugEnabled.Value)
                {
                    OnDrawGizmos(__instance);
                }
            }
        }
    }
}
