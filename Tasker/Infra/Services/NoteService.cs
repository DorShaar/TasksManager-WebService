using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.Notes;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.Infra.Services
{
    public class NoteService : INoteService
    {
        private const string TextFileExtension = ".txt";

        private readonly string mGeneralNotesDirectory;
        private readonly string mTasksNotesDirectory;
        private readonly INoteFactory mNoteFactory;
        private readonly ILogger<NoteService> mLogger;

        public NoteService(INoteFactory noteFactory, IOptions<DatabaseConfigurtaion> options, ILogger<NoteService> logger)
        {
            mNoteFactory = noteFactory ?? throw new ArgumentNullException(nameof(noteFactory));
            mGeneralNotesDirectory = options.Value.NotesDirectoryPath ??
                throw new ArgumentNullException(nameof(options.Value.NotesDirectoryPath));
            mTasksNotesDirectory = options.Value.NotesTasksDirectoryPath ??
                throw new ArgumentNullException(nameof(options.Value.NotesTasksDirectoryPath));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IResponse<INote>> GetGeneralNote(string noteIdentifier)
        {
            mLogger.LogDebug($"Creating notes file system structure from {mGeneralNotesDirectory}");

            noteIdentifier = AddExtensionIfNotExists(noteIdentifier);

            noteIdentifier = RemoveDuplicateDirectory(noteIdentifier);

            INote note = mNoteFactory.LoadNote(Path.Combine(mGeneralNotesDirectory, noteIdentifier));

            if (!File.Exists(note.NotePath))
                return new FailResponse<INote>($"No note found in path {note.NotePath}");

            return new SuccessResponse<INote>(note);
        }

        public async Task<IResponse<INote>> GetTaskNote(string noteIdentifier)
        {
            mLogger.LogDebug($"Creating notes file system structure from {mGeneralNotesDirectory}");

            string taskNotePath = Directory.EnumerateFiles(mTasksNotesDirectory).FirstOrDefault(
                note => Path.GetFileName(note).StartsWith(noteIdentifier));

            if (taskNotePath == null)
                return new FailResponse<INote>($"No note {noteIdentifier} found");

            INote note = mNoteFactory.LoadNote(taskNotePath);

            return new SuccessResponse<INote>(note);
        }

        private string AddExtensionIfNotExists(string fileName)
        {
            if (!Path.GetExtension(fileName).Equals(TextFileExtension))
                fileName += TextFileExtension;

            return fileName;
        }

        private string RemoveDuplicateDirectory(string fileName)
        {
            string fileNameWithoutDuplicatedDirectory = fileName;

            string notesDirectoryName = Path.GetFileName(mGeneralNotesDirectory);

            if (fileName.StartsWith(notesDirectoryName) &&
                !Directory.GetDirectories(mGeneralNotesDirectory).Any(
                    directoryPath => Path.GetFileName(directoryPath) == notesDirectoryName))
            {
                fileNameWithoutDuplicatedDirectory = fileName.Remove(0, notesDirectoryName.Length);
                fileNameWithoutDuplicatedDirectory =
                    fileNameWithoutDuplicatedDirectory.TrimStart(Path.DirectorySeparatorChar);
            }

            return fileNameWithoutDuplicatedDirectory;
        }

        public Task<NoteNode> GetNotesStructure()
        {
            mLogger.LogDebug($"Creating notes file system structure from {mGeneralNotesDirectory}");
            return Task.FromResult(new NoteNode(mGeneralNotesDirectory));
        }
    }
}