using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tasker.Domain.Extensions;

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
            await FindRecursive(name.Split(Path.DirectorySeparatorChar), pathOfSameNames).ConfigureAwait(false);

            return pathOfSameNames;
        }

        private Task FindRecursive(string[] pathComponents, List<string> pathOfSameNames)
        {
            ValidateParametersInput(pathComponents, pathOfSameNames);

            if (Children.Count == 0 && pathComponents.Length == 1 && AreSameName(pathComponents[0], Name))
            {
                pathOfSameNames.Add(mPath);
                return Task.CompletedTask;
            }

            int taskIndex = 0;
            Task[] tasks = new Task[Children.Count];

            foreach (NoteNode noteNode in Children.Values)
            {
                if (AreSameName(pathComponents[0], Name))
                    tasks[taskIndex] = noteNode.FindRecursive(pathComponents.Slice(1, pathComponents.Length), pathOfSameNames);
                else
                    tasks[taskIndex] = noteNode.FindRecursive(pathComponents, pathOfSameNames);

                taskIndex++;
            }

            if (!Task.WaitAll(tasks, TimeoutThreshold))
                throw new TimeoutException($"Search exceeded limit of {TimeoutThreshold.TotalSeconds}");

            return Task.CompletedTask;
        }

        private void ValidateParametersInput(string[] pathComponents, List<string> pathOfSameNames)
        {
            if (pathOfSameNames == null)
                throw new ArgumentNullException(nameof(pathOfSameNames));

            if (pathComponents == null)
                throw new ArgumentNullException(nameof(pathComponents));

            if (pathComponents.Length == 0)
                throw new ArgumentException($"{nameof(pathComponents)} cannot be empty");
        }

        private bool AreSameName(string name1, string name2)
        {
            if (Path.HasExtension(name1) && Path.HasExtension(name2))
                return name1.Equals(name2, StringComparison.InvariantCultureIgnoreCase);

            return Path.GetFileNameWithoutExtension(name1).Equals(
                Path.GetFileNameWithoutExtension(name2), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}