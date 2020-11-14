using System.Collections.Generic;
using System.Linq;
using TaskData.Notes;

namespace Tasker.App.Resources
{
    public class NoteResource
    {
        public bool IsNoteFound => PossibleNotes.Any();
        public bool IsMoreThanOneNoteFound => PossibleNotes.Count() > 1;
        public IEnumerable<string> PossibleNotes { get; }
        public INote Note { get; }

        public NoteResource(IEnumerable<string> notes, INoteFactory noteFactory)
        {
            PossibleNotes = notes;

            if (notes.Count() == 1)
            {
                Note = noteFactory.LoadNote(PossibleNotes.First());
                return;
            }

            Note = noteFactory.LoadNote(string.Empty);
        }
    }
}