using System.Collections.Generic;
using System.IO;

namespace Tasker.Domain.Models
{
    public class NoteNode
    {
        public string Name { get; }
        public Dictionary<string, NoteNode> Children { get; } = new Dictionary<string, NoteNode>();

        public NoteNode(string nodePath)
        {
            Name = Path.GetFileName(nodePath);

            if (!Directory.Exists(nodePath))
                return;
            
            foreach (string filePath in Directory.EnumerateFileSystemEntries(nodePath))
            {
                NoteNode newChildNote = new NoteNode(filePath);
                Children.Add(newChildNote.Name, newChildNote);
            }
        }
    }
}