using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffReducer
{
    public static class BeatmapSimplifier
    {
        public static Tuple<float,float,List<BeatmapObjectData>> SimplifyBeatmap(BeatmapData beatmap, float bpm)
        {
            float beatDivision = UI.ModifierUI.instance.beatDivision;
            float bps = 60f / bpm;
            float maxTimeDiff = beatDivision <= 0? float.MaxValue : bps / beatDivision;
            float secondaryMaxTimeDiff = bps / UI.ModifierUI.instance.beatDivision2;
            var mapNotes = new List<NoteData>();
            var objects = beatmap.allBeatmapDataItems.Where(x => x is BeatmapObjectData).Cast<BeatmapObjectData>();
            foreach (var beatmapObject in objects)
                if (beatmapObject is NoteData) mapNotes.Add(beatmapObject as NoteData);
            mapNotes.RemoveAll(x => x.colorType == ColorType.None);
            mapNotes = mapNotes.OrderBy(x => x.time).ToList();
            var objectsWithoutNotes = objects.Where(x => !x.IsNote());
            List<NoteSection> mapData = new List<NoteSection>();
            NoteSection lastSection = new NoteSection(bps, secondaryMaxTimeDiff);
            foreach (var note in mapNotes)
            {
                NoteData lastNote = lastSection.LastNote();
                if (lastNote == null)
                    lastSection.AddNote(note);
                else
                {
                    float timeDiff = note.time - lastNote.time;
                    if (timeDiff <= maxTimeDiff)
                        lastSection.AddNote(note);
                    else
                    {
                        lastSection.CalculateSwings();
                        if (lastSection.swingCount >= 4)
                        {
                            mapData.Add(lastSection);
                        }
                        else
                            objectsWithoutNotes = objectsWithoutNotes.Concat(lastSection.GetBaseNotes());

                        lastSection = new NoteSection(maxTimeDiff, secondaryMaxTimeDiff);
                        lastSection.AddNote(note);
                    }
                }
            }
            lastSection.CalculateSwings();
            if (lastSection.swingCount >= 4)
                mapData.Add(lastSection);
            else
                objectsWithoutNotes = objectsWithoutNotes.Concat(lastSection.GetBaseNotes());
            mapData = mapData.OrderBy(x => x.FirstNote().time).ToList();
            //Simplify map
            var newMap = objectsWithoutNotes.ToList();
            float initialNPS = objects.Count() > 0 ? objects.Where(x => x.IsNote()).Count() / objects.Last().time : 0;
          //  Plugin.log.Debug($"Initial NPS: {initialNPS}");
            foreach (var section in mapData)
            {
         //       Plugin.log.Debug($"Initial section swings: {section.swingCount}");
                section.SimplifySwings();
        //        Plugin.log.Debug($"Final section swings: {section.swingCount}");
                newMap.AddRange(section.GetFinalNotes());
            }
            float finalNPS = objects.Count() > 0 ? newMap.Where(x => x.IsNote()).Count() / objects.Last().time : 0;
       //     Plugin.log.Debug($"Final NPS: {finalNPS}");
            return new Tuple<float, float, List<BeatmapObjectData>>(initialNPS, finalNPS, newMap);
        }
    }
}
