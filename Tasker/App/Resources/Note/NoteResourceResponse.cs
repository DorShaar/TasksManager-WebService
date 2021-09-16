using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskData.Notes;

namespace Tasker.App.Resources.Note
{
    public class NoteResourceResponse
    {
        public bool IsNoteFound => PossibleNotesRelativePaths.Any();
        public bool IsMoreThanOneNoteFound => PossibleNotesRelativePaths.Count() > 1;
        public IEnumerable<string> PossibleNotesRelativePaths { get; }
        public INote Note { get; }

        public NoteResourceResponse(string baseDirectory,
            IEnumerable<string> notes)
        {
            PossibleNotesRelativePaths = notes.Select(
                path => Path.GetRelativePath(baseDirectory, path));

            if (notes.Count() == 1)
            {
                Note = null;
                return;
            }

            Note = null;
        }
    }
}