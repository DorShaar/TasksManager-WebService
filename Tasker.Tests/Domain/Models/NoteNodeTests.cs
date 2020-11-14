using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tasker.Domain.Models;
using Xunit;

namespace Tasker.Tests.Domain.Models
{
    public class NoteNodeTests
    {
        private const string TestFilesDirectory = "TestFiles";
        private const string GeneralNotesDirectoryName = "GeneralNotes";

        [Fact]
        public void Ctor_CreatedAsExpected()
        {
            NoteNode noteNode = new NoteNode(Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName));

            Assert.Equal(GeneralNotesDirectoryName, noteNode.Name);
            Assert.Equal(3, noteNode.Children.Count);

            Assert.Equal("subject1", noteNode.Children["subject1"].Name);
            Assert.Equal(2, noteNode.Children["subject1"].Children.Count);
            Assert.Equal("generalNote3.txt", noteNode.Children["generalNote3.txt"].Name);
            Assert.Empty(noteNode.Children["generalNote3.txt"].Children);

            Assert.Equal("generalNote1.txt", noteNode.Children["subject1"].Children["generalNote1.txt"].Name);
            Assert.Equal("generalNote2.txt", noteNode.Children["subject1"].Children["generalNote2.txt"].Name);
        }

        [Fact]
        public async Task FindRecursive_HasOneExactly_ReturnsOnePath()
        {
            NoteNode noteNode = new NoteNode(Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName));

            IEnumerable<string> path = await noteNode.FindRecursive("generalNote2").ConfigureAwait(false);

            Assert.Single(path);
            Assert.Equal(Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName, "subject1", "generalNote2.txt"), path.First());
        }

        [Fact]
        public async Task FindRecursive_HasTwoPaths_ReturnsTwoPaths()
        {
            NoteNode noteNode = new NoteNode(Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName));

            IEnumerable<string> paths = await noteNode.FindRecursive("generalNote1").ConfigureAwait(false);
            List<string> pathsList = paths.ToList();

            Assert.Equal(2, pathsList.Count);
            Assert.Equal(Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName, "subject1", "generalNote1.txt"), pathsList[0]);
            Assert.Equal(Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName, "subject2", "generalNote1.txt"), pathsList[1]);
        }

        [Fact]
        public async Task FindRecursive_NotExists_ReturnsEmpty()
        {
            NoteNode noteNode = new NoteNode(Path.Combine(TestFilesDirectory, GeneralNotesDirectoryName));

            IEnumerable<string> paths = await noteNode.FindRecursive("NoteExistsNote").ConfigureAwait(false);

            Assert.Empty(paths);
        }
    }
}