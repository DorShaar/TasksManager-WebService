using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tasker.Domain.Models
{
    public class NoteNode
    {
        private static readonly TimeSpan TimeoutThreshold = TimeSpan.FromSeconds(10);

        private readonly string mPath;
        public string Name { get; }
        public Dictionary<string, NoteNode> Children { get; } = new Dictionary<string, NoteNode>();

        public NoteNode(string nodePath)
        {
            mPath = nodePath;
            Name = Path.GetFileName(nodePath);

            if (!Directory.Exists(nodePath))
                return;

            foreach (string filePath in Directory.EnumerateFileSystemEntries(nodePath))
            {
                NoteNode newChildNote = new NoteNode(filePath);
                Children.Add(newChildNote.Name, newChildNote);
            }
        }

        public async Task<IEnumerable<string>> FindRecursive(string name)
        {
            List<string> pathOfSameNames = new List<string>();
            await FindRecursive(name, pathOfSameNames).ConfigureAwait(false);

            return pathOfSameNames;
        }

        private Task FindRecursive(string name, List<string> pathOfSameNames)
        {
            if (pathOfSameNames == null)
                throw new ArgumentNullException(nameof(pathOfSameNames));

            if (Children.Count == 0 && Path.GetFileNameWithoutExtension(Name).Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                pathOfSameNames.Add(mPath);
                return Task.CompletedTask;
            }

            int taskIndex = 0;
            Task[] tasks = new Task[Children.Count];

            foreach (NoteNode noteNode in Children.Values)
            {
                tasks[taskIndex] = noteNode.FindRecursive(name, pathOfSameNames);
                taskIndex++;
            }

            if (!Task.WaitAll(tasks, TimeoutThreshold))
                throw new TimeoutException($"Search exceeded limit of {TimeoutThreshold.TotalSeconds}");

            return Task.CompletedTask;
        }
    }
}