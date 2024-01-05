using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using HMUI;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
using UnityEngine;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage.Util;

namespace DiffReducer.UI
{
    class ModifierUI : NotifiableSingleton<ModifierUI>
    {
        public IDifficultyBeatmap currentBeatmap { get; private set; } = null;
        public List<BeatmapObjectData> simplifiedMap { get; private set; } = null;
        private string _initialNPSText = "Initial NPS: --";
        [UIValue("initialNPS")]
        public string initialNPSText
        {
            get => "<#ebbd52>" + _initialNPSText;
            set
            {
                _initialNPSText = value;
                NotifyPropertyChanged();
            }
        }
        private string _finalNPSText = "New NPS: --";
        [UIValue("finalNPS")]
        public string finalNPSText
        {
            get => "<#ebbd52>" + _finalNPSText;
            set
            {
                _finalNPSText = value;
                NotifyPropertyChanged();
            }
        }
        [UIValue("warningText")]
        internal string warning = "<#BB1111>Warning\nWhen this is active the map is modified and is no longer representative of the quality of the actual map";
        private bool _enabled = false;
        [UIValue("enabled")]
        public bool modEnabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                NotifyPropertyChanged();
            }
        }
        [UIAction("setEnabled")]
        void setEnabled(bool value)
        {
            modEnabled = value;
        }
        private float _beatDivision = 1.5f;
        [UIValue("beatDivision")]
        public float beatDivision
        {
            get => _beatDivision;
            set
            {
                _beatDivision = value;
                UpdateSimplifiedBeatmap();
                NotifyPropertyChanged();
            }
        }
        [UIAction("setBeatDivision")]
        void setBeatDivision(float value)
        {
            beatDivision = value;
            Config.Write();
        }

        private float _beatDivision2 = 1.5f;
        [UIValue("beatDivision2")]
        public float beatDivision2
        {
            get => _beatDivision2;
            set
            {
                _beatDivision2 = value;
                UpdateSimplifiedBeatmap();
                NotifyPropertyChanged();
            }
        }
        [UIAction("setBeatDivision2")]
        void setBeatDivision2(float value)
        {
            beatDivision2 = value;
            Config.Write();
        }
        public void UpdateSimplifiedMap(float initialNPS, float newNPS, List<BeatmapObjectData> newMap)
        {
            simplifiedMap = newMap;
            initialNPSText = $"Initial NPS: {initialNPS.ToString("F2")}";
            finalNPSText = $"New NPS: {newNPS.ToString("F2")}";
        }
        private bool _simplifySwingLength = false;
        [UIValue("simplifySwingLength")]
        public bool simplifySwingLength
        {
            get => _simplifySwingLength;
            set
            {
                _simplifySwingLength = value;
                UpdateSimplifiedBeatmap();
                NotifyPropertyChanged();
            }
        }
        [UIAction("setSimplifySwingLength")]
        void setSimplifySwingLength(bool value)
        {
            simplifySwingLength = value;
            Config.Write();
        }
        //  public static void DidChangeDiffBeatmap(LevelCollectionNavigationController arg1, IDifficultyBeatmap arg2)
        //  {
        //      var result = BeatmapSimplifier.SimplifyBeatmap(arg2.beatmapData, arg2.level.beatsPerMinute);
        //      ModifierUI.instance.UpdateSimplifiedMap(result.Item1, result.Item2, result.Item3);
        //  }
        public static void Leveldetail_didChangeContentEvent(StandardLevelDetailViewController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
            if (arg1 != null && arg1.selectedDifficultyBeatmap != null)
            {
                if(arg1.selectedDifficultyBeatmap != ModifierUI.instance.currentBeatmap)
                {
                    ModifierUI.instance.currentBeatmap = arg1.selectedDifficultyBeatmap;
                    ModifierUI.instance.UpdateSimplifiedBeatmap();
                }
            }
        }
        public async void UpdateSimplifiedBeatmap()
        {
            if (currentBeatmap == null) return;
            var result = BeatmapSimplifier.SimplifyBeatmap(await currentBeatmap.GetBeatmapDataAsync(currentBeatmap.level.environmentInfo, null) as BeatmapData, currentBeatmap.level.beatsPerMinute);
            ModifierUI.instance.UpdateSimplifiedMap(result.initialNps, result.finalNps, result.newMap);
        }

    }
}
