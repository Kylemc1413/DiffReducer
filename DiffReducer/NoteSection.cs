using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace DiffReducer
{
    public class NoteSection
    {
        List<NoteData> _baseNotes = new List<NoteData>();
        private float beatsPerSecond = 0;
        private float secondaryMaxTimeDiff = 0;
        List<NoteSwing> _swings = new List<NoteSwing>();
        public int swingCount => _swings.Count;

        public NoteSection(float timeDiff, float timeDiff2 = 0)
        {
            beatsPerSecond = timeDiff;
            if (timeDiff2 == 0)
                secondaryMaxTimeDiff = beatsPerSecond;
            else
                secondaryMaxTimeDiff = timeDiff2;
        }
        public void AddNote(NoteData note)
        {
            _baseNotes.Add(note);
        }

        public NoteData FirstNote()
        {
            return _baseNotes.FirstOrDefault();
        }
        public NoteData LastNote()
        {
            return _baseNotes.LastOrDefault();
        }

        public void CalculateSwings()
        {
            var leftNotes = _baseNotes.Where(x => x.colorType == ColorType.ColorA);
            var rightNotes = _baseNotes.Where(x => x.colorType == ColorType.ColorB);
            //Calculate Swings
            NoteSwing swing = new NoteSwing();
            float maxSwingTimeDiff = beatsPerSecond / 2f;
            if (leftNotes.Count() > 0)
            {
                swing.AddNote(leftNotes.First());
                if (leftNotes.Count() > 1)
                    for (int i = 1; i < leftNotes.Count(); i++)
                    {
                        var nextNote = leftNotes.ElementAt(i);
                        float timeDiff = nextNote.time - swing.firstNote.time;
                        if (timeDiff <= maxSwingTimeDiff && !nextNote.IsContraryDirectionWithinSwing(swing.firstNote))
                        {
                            swing.AddNote(nextNote);
                        }
                        else
                        {
                            _swings.Add(swing);
                            swing = new NoteSwing();
                            swing.AddNote(nextNote);
                        }
                    }
            }
            _swings.Add(swing);
            swing = new NoteSwing();
            if (rightNotes.Count() > 0)
            {
                swing.AddNote(rightNotes.First());
                if (rightNotes.Count() > 1)
                    for (int i = 1; i < rightNotes.Count(); i++)
                    {
                        var nextNote = rightNotes.ElementAt(i);
                        float timeDiff = nextNote.time - swing.firstNote.time;
                        if (timeDiff <= maxSwingTimeDiff && !nextNote.IsContraryDirectionWithinSwing(swing.firstNote))
                        {
                            swing.AddNote(nextNote);
                        }
                        else
                        {
                            _swings.Add(swing);
                            swing = new NoteSwing();
                            swing.AddNote(nextNote);
                        }
                    }
            }
            _swings.Add(swing);
            _swings.RemoveAll(x => x.firstNote == null);
        }

        public void SimplifySwings()
        {
            var orderedSwings = _swings.OrderBy(x => x.firstNote.time).ToList();
            var finalSwings = new List<NoteSwing>();
            var firstSwing = orderedSwings.First();
            var lastSwing = orderedSwings.Last();
            finalSwings.Add(orderedSwings.First());
            int state = 0;
            for (int i = 1; i < orderedSwings.Count() - 1; ++i)
            {
                NoteSwing swing = orderedSwings.ElementAt(i);
                NoteSwing prevHandSwing = finalSwings.LastOrDefault
               (x => x.firstNote.time < swing.firstNote.time && x.firstNote.colorType == swing.firstNote.colorType);
                NoteSwing nextHandSwing = orderedSwings.FirstOrDefault
                    (x => x.firstNote.time > swing.firstNote.time && x.firstNote.colorType == swing.firstNote.colorType);
                //      var doubleSwing = orderedSwings.FirstOrDefault(
                //          x => !finalSwings.Contains(x) &&
                //          Mathf.Abs(x.firstNote.time - swing.firstNote.time) <= 0.0001 &&
                //          x.firstNote.colorType != swing.firstNote.colorType);
                if (!finalSwings.Contains(swing))
                    switch (state)
                    {

                        case 0:
                        case 1:
                        case 2:
                            if (prevHandSwing != null)
                            {
                                if (!swing.SwingDirection.IsContraryDirectionBetweenSwings(prevHandSwing.SwingDirection))
                                {
                                    float timeDiff = swing.firstNote.time - prevHandSwing.firstNote.time;
                                    if (timeDiff > secondaryMaxTimeDiff)
                                    {
                                        finalSwings.Add(swing);
                                        //            if (doubleSwing != null)
                                        //               finalSwings.Add(doubleSwing);
                                        //   state = -1;
                                    }
                                }
                            }
                            else
                            {
                                finalSwings.Add(swing);
                                //        if (doubleSwing != null)
                                //         finalSwings.Add(doubleSwing);
                            }
                            if (state == 2)
                                state = -1;
                            break;
                        /*
                    case 1:
                        if (prevHandSwing != null)
                        {
                            if (swing.SwingDirection.IsContraryDirectionBetweenSwings(prevHandSwing.SwingDirection))
                            {
                                //    swing.FullMirrorSwing();
                                //              swing.MirrorDirectionSwing();
                                if (nextHandSwing == null)
                                {
                                    finalSwings.Add(swing);
                                    if (doubleSwing != null)
                                        finalSwings.Add(doubleSwing);
                                }
                                break;
                            }
                        }
                        finalSwings.Add(swing);
                        if (doubleSwing != null)
                            finalSwings.Add(doubleSwing);
                        break;
                        */
                        default:
                            break;
                    }
                ++state;
            }
            finalSwings.Add(orderedSwings.Last());
            finalSwings = finalSwings.OrderBy(x => x.firstNote.time).ToList();
            _swings = finalSwings;
        }
        public IEnumerable<BeatmapObjectData> GetBaseNotes()
        {
            return _baseNotes;
        }
        public IEnumerable<BeatmapObjectData> GetFinalNotes()
        {
            List<NoteData> finalNotes = new List<NoteData>();
            foreach (var swing in _swings)
            {
                if (UI.ModifierUI.instance.simplifySwingLength)
                    finalNotes.Add(swing.firstNote);
                else
                    finalNotes.AddRange(swing.GetNotes());
            }

            return finalNotes;
        }
    }

}
