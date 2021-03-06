﻿using System;
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

        private async Task FindRecursive(string[] pathComponents, List<string> pathOfSameNames)
        {
            ValidateParametersInput(pathComponents, pathOfSameNames);

            if (pathComponents.Length == 0)
                return;

            if (Children.Count == 0 && pathComponents.Length == 1 && AreSameName(pathComponents[0], Name))
            {
                pathOfSameNames.Add(mPath);
                return;
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
        }

        private void ValidateParametersInput(string[] pathComponents, List<string> pathOfSameNames)
        {
            if (pathOfSameNames == null)
                throw new ArgumentNullException(nameof(pathOfSameNames));

            if (pathComponents == null)
                throw new ArgumentNullException(nameof(pathComponents));
        }

        private bool AreSameName(string name1, string name2)
        {
            if (Path.HasExtension(name1) && Path.HasExtension(name2))
                return name1.Equals(name2, StringComparison.InvariantCultureIgnoreCase);

            string fileName1WithoutExtension = Path.GetFileNameWithoutExtension(name1);
            string fileName2WithoutExtension = Path.GetFileNameWithoutExtension(name2);

            if (string.IsNullOrWhiteSpace(fileName1WithoutExtension) || string.IsNullOrWhiteSpace(fileName2WithoutExtension))
                return false;

            if (name1.Length > name2.Length)
            {
                return Path.GetFileNameWithoutExtension(name1).Contains(
                    Path.GetFileNameWithoutExtension(name2), StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return Path.GetFileNameWithoutExtension(name2).Contains(
                    Path.GetFileNameWithoutExtension(name1), StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}