using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using IPA.Utilities;
namespace DiffReducer
{

    [HarmonyPatch(typeof(BeatmapDataTransformHelper))]
    [HarmonyPatch("CreateTransformedBeatmapData")]
    class DiffReductionPatch
    {
        static void Postfix(IReadonlyBeatmapData beatmapData, ref IReadonlyBeatmapData __result, bool leftHanded)
        {
            if (!BS_Utils.Plugin.LevelData.IsSet || BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Standard || !UI.ModifierUI.instance.modEnabled) return;

            BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("DiffReducer");

            __result = beatmapData.GetFilteredCopy(x =>
            {
                
                if (x is BeatmapObjectData && !UI.ModifierUI.instance.simplifiedMap.Any(y => x.CompareTo(y) == 0))
                    return null;
                return x;
            });
        }
    }
}
