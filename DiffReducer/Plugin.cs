using IPA;
using IPALogger = IPA.Logging.Logger;
using UnityEngine;
using System.Linq;
namespace DiffReducer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger log { get; set; }

        [Init]
        public Plugin(IPALogger logger)
        {
            Instance = this;
            log = logger;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            var harmony = new HarmonyLib.Harmony("com.kyle1413.beatsaber.diffreducer");
            harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
            BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("DiffReducer", "DiffReducer.UI.modifierUI.bsml", UI.ModifierUI.instance, BeatSaberMarkupLanguage.GameplaySetup.MenuType.Solo);
            BS_Utils.Utilities.BSEvents.lateMenuSceneLoadedFresh += BSEvents_lateMenuSceneLoadedFresh;
            Config.Read();
        }

        private void BSEvents_lateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
      //      var levelnav = Resources.FindObjectsOfTypeAll<LevelCollectionNavigationController>().FirstOrDefault();
            var leveldetail = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
       //     if(levelnav != null)
       //     {
       //         levelnav.didChangeDifficultyBeatmapEvent += UI.ModifierUI.DidChangeDiffBeatmap;
       //     }
            if(leveldetail != null)
            {
                leveldetail.didChangeContentEvent += UI.ModifierUI.Leveldetail_didChangeContentEvent;
            }
        }

        private void Levelnav_didChangeLevelDetailContentEvent(LevelCollectionNavigationController arg1, StandardLevelDetailViewController.ContentType arg2)
        {
        }

        [OnExit]
        public void OnApplicationQuit()
        {

        }

    }
}
