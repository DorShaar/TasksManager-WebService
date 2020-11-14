using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.Notes;
using Tasker.App.Resources.Note;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.Infra.Services
{
    public class NoteService : INoteService
    {
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

        public async Task<IResponse<NoteResourceResponse>> GetGeneralNote(string noteIdentifier)
        {
            NoteNode generalNotesStructure = await GetNotesStructure().ConfigureAwait(false);

            IEnumerable<string> notePaths = await generalNotesStructure.FindRecursive(noteIdentifier).ConfigureAwait(false);

            NoteResourceResponse noteResponse = new NoteResourceResponse(notePaths, mNoteFactory);

            if (!noteResponse.IsNoteFound)
                return new FailResponse<NoteResourceResponse>($"No {noteIdentifier} note found");

            return new SuccessResponse<NoteResourceResponse>(noteResponse);
        }

        public async Task<IResponse<NoteResourceResponse>> GetTaskNote(string noteIdentifier)
        {
            mLogger.LogDebug($"Creating notes file system structure from {mTasksNotesDirectory}");
            NoteNode tasksNotesStructure = new NoteNode(mTasksNotesDirectory);

            IEnumerable<string> notePaths = await tasksNotesStructure.FindRecursive(noteIdentifier).ConfigureAwait(false);

            NoteResourceResponse noteResponse = new NoteResourceResponse(notePaths, mNoteFactory);

            if (!noteResponse.IsNoteFound)
                return new FailResponse<NoteResourceResponse>($"No {noteIdentifier} note found");

            return new SuccessResponse<NoteResourceResponse>(noteResponse);
        }

        public Task<NoteNode> GetNotesStructure()
        {
            mLogger.LogDebug($"Creating notes file system structure from {mGeneralNotesDirectory}");
            return Task.FromResult(new NoteNode(mGeneralNotesDirectory));
        }
    }
}