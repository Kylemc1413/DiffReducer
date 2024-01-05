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

            var simplifiedBeatmap = BeatmapSimplifier.SimplifyBeatmap(__result as BeatmapData, UI.ModifierUI.instance.currentBeatmap.level.beatsPerMinute);
            __result = __result.GetFilteredCopy(x =>
            {
                
                if (x is BeatmapObjectData && !simplifiedBeatmap.newMap.Any(y => x.CompareTo(y) == 0))
                    return null;
                return x;
            });
        }
    }
}
