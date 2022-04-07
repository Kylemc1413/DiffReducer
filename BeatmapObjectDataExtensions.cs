using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffReducer
{
    public static class BeatmapObjectDataExtensions
    {
        public static bool IsNote(this BeatmapObjectData data)
        {
            return data is NoteData note && note.gameplayType != NoteData.GameplayType.Bomb;
        }
    }
}
