using System.IO;
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
            Assert.Equal(2, noteNode.Children.Count);

            Assert.Equal("subject1", noteNode.Children["subject1"].Name);
            Assert.Equal(2, noteNode.Children["subject1"].Children.Count);
            Assert.Equal("generalNote3.txt", noteNode.Children["generalNote3.txt"].Name);
            Assert.Empty(noteNode.Children["generalNote3.txt"].Children);
            
            Assert.Equal("generalNote1.txt", noteNode.Children["subject1"].Children["generalNote1.txt"].Name);
            Assert.Equal("generalNote2.txt", noteNode.Children["subject1"].Children["generalNote2.txt"].Name);
        }
    }
}