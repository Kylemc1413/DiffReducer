using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IPA.Utilities;
namespace DiffReducer
{
    class NoteSwing
    {
        int SwingLength => _notes.Count;
        public NoteCutDirection SwingDirection { get; private set; } = NoteCutDirection.None;

        List<NoteData> _notes = new List<NoteData>();
        public NoteData firstNote => _notes.FirstOrDefault();
        public void AddNote(NoteData note)
        {
            if (_notes.Count == 0)
                SwingDirection = note.cutDirection;
            _notes.Add(note);
        }
        public IEnumerable<NoteData> GetNotes()
        {
            return _notes;
        }
        public void FullMirrorSwing()
        {
            for (int i = 0; i < _notes.Count; i++)
            {
                NoteData note = _notes[i];
                note.Mirror(4);
            }
        }
        public void MirrorDirectionSwing()
        {
            for (int i = 0; i < _notes.Count; i++)
            {
                NoteData note = _notes[i];
               note.ChangeNoteCutDirection(note.cutDirection.Mirrored());
            }
        }
        public void SwapHandSwing()
        {
            for (int i = 0; i < _notes.Count; i++)
            {
                NoteData note = _notes[i];
                note.SetProperty("colorType", note.colorType.Opposite());
            }
        }
    }
    static class AdditionalNoteExtensions
    {
        public static bool IsContraryDirectionWithinSwing(this NoteData note, NoteData prevNote)
        {
            if (note.cutDirection == NoteCutDirection.Any) return false;
            else
            {
                float angleDiff = note.cutDirection.RotationAngle() - prevNote.cutDirection.RotationAngle();
                if (Mathf.Abs(angleDiff) > 45f)
                    return true;
                else
                    return false;
            }
        }
        public static bool IsContraryDirectionBetweenSwings(this NoteData note, NoteCutDirection prevCutDirection)
        {
            if (note.cutDirection == NoteCutDirection.Any) return false;
            else
            {
                float angleDiff = note.cutDirection.RotationAngle() - prevCutDirection.RotationAngle();
                if (Mathf.Abs(angleDiff) < 90f)
                    return true;
                else
                    return false;
            }
        }
        public static bool IsContraryDirectionBetweenSwings(this NoteCutDirection cutDirection, NoteCutDirection prevCutDirection)
        {
            if (cutDirection == NoteCutDirection.Any) return false;
            else
            {
                float angleDiff = cutDirection.RotationAngle() - prevCutDirection.RotationAngle();
                if (Mathf.Abs(angleDiff) < 90f)
                    return true;
                else
                    return false;
            }
        }
    }

}
