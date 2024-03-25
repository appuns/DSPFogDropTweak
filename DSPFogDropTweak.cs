using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]



namespace DSPFogDropTweak
{
    [BepInPlugin("Appun.DSP.plugin.FogDropTweak", "DSPFogDropTweak", "0.0.1")]
    [HarmonyPatch]
	public class Main : BaseUnityPlugin
	{

        public static ConfigEntry<int> DropRatioMultiplier;
        public static ConfigEntry<int> DropCountMultiplier;
        public static ConfigEntry<bool> DisablePlanetDropCorrection;

        public void Awake()
        {
            DropRatioMultiplier = Config.Bind("General", "DropRatioMultiplier", 1, "Multiply the drop ratio.");
            DropCountMultiplier = Config.Bind("General", "DropCountMultiplier", 1, "Multiply  the drop count.");
            DisablePlanetDropCorrection = Config.Bind("General", "DisablePlanetDropCorrection", true, "Disable planet drop correction.");

            LogManager.Logger = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

		}

        [HarmonyPostfix, HarmonyPatch(typeof(ItemProto), "InitEnemyDropTables")]
        public static void ItemProto_InitEnemyDropTables_Postfix(ItemProto __instance)
        {

            ItemProto[] dataArray = LDB.items.dataArray;
            for (int j = 0; j < dataArray.Length; j++)
            {
                int id = dataArray[j].ID;
                if (dataArray[j].EnemyDropRange.y > 5E-05f)
                {
                    dataArray[j].EnemyDropRange.y *= DropRatioMultiplier.Value; //ドロップ率
                    //ItemProto.enemyDropLevelTable[id] = dataArray[j].EnemyDropLevel; //ドロップレベル
                    dataArray[j].EnemyDropCount *= DropCountMultiplier.Value; //ドロップ数
                    ItemProto.enemyDropCountTable[id] = dataArray[j].EnemyDropCount;
                    if(DisablePlanetDropCorrection.Value)
                    {
                        ItemProto.enemyDropMaskTable[id] = 2147483647; //マスク　2147483647=全ての惑星タイプでドロップ
                    }
                    //ItemProto.enemyDropMaskRatioTable[id] = 1f; //補正倍率
                    //LogManager.Logger.LogInfo($"{LDB.items.Select(id).name}:{ItemProto.enemyDropLevelTable[id]}:{ItemProto.enemyDropCountTable[id]}:{ItemProto.enemyDropMaskTable[id]}:{ItemProto.enemyDropMaskRatioTable[id]}");
                }
            }
        }

        public class LogManager
        {
            public static ManualLogSource Logger;
        }
    }
}