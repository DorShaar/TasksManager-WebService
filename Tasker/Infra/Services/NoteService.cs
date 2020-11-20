using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Takser.Infra.Options;
using TaskData.Notes;
using Tasker.App.Resources.Note;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;
using Tasker.Infra.Consts;

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
            NoteNode generalNotesStructure = await GetGeneralNotesStructure().ConfigureAwait(false);

            IEnumerable<string> notesPaths = await generalNotesStructure.FindRecursive(noteIdentifier).ConfigureAwait(false);

            NoteResourceResponse noteResponse = new NoteResourceResponse(
                mGeneralNotesDirectory, notesPaths, mNoteFactory);

            if (!noteResponse.IsNoteFound)
                return new FailResponse<NoteResourceResponse>($"No {noteIdentifier} note found");

            return new SuccessResponse<NoteResourceResponse>(noteResponse);
        }

        public async Task<IResponse<NoteResourceResponse>> GetTaskNote(string noteIdentifier)
        {
            mLogger.LogDebug($"Creating notes file system structure from {mTasksNotesDirectory}");
            NoteNode tasksNotesStructure = new NoteNode(mTasksNotesDirectory);

            IEnumerable<string> notesPaths = await tasksNotesStructure.FindRecursive(noteIdentifier).ConfigureAwait(false);

            NoteResourceResponse noteResponse = new NoteResourceResponse(
                mTasksNotesDirectory, notesPaths, mNoteFactory);

            if (!noteResponse.IsNoteFound)
                return new FailResponse<NoteResourceResponse>($"No {noteIdentifier} note found");

            return new SuccessResponse<NoteResourceResponse>(noteResponse);
        }

        public Task<NoteNode> GetGeneralNotesStructure()
        {
            mLogger.LogDebug($"Creating notes file system structure from {mGeneralNotesDirectory}");
            return Task.FromResult(new NoteNode(mGeneralNotesDirectory));
        }

        public Task<NoteNode> GetNotesStructure()
        {
            mLogger.LogDebug($"Creating notes file system structure from {mTasksNotesDirectory}");
            return Task.FromResult(new NoteNode(mTasksNotesDirectory));
        }

        public async Task<IResponse<NoteResourceResponse>> CreatePrivateNote(string notePath, string text)
        {
            mLogger.LogDebug($"Creating private note {notePath}");

            string fixedNotePath = GetFixedPrivateNotePath(notePath);

            if (File.Exists(fixedNotePath))
                return new FailResponse<NoteResourceResponse>($"File {notePath} is already exist");

            try
            {
                await File.WriteAllTextAsync(fixedNotePath, text).ConfigureAwait(false);

                NoteResourceResponse noteResponse = new NoteResourceResponse(
                    mTasksNotesDirectory, new string[] { fixedNotePath }, mNoteFactory);

                return new SuccessResponse<NoteResourceResponse>(noteResponse);
            }
            catch (Exception ex)
            {
                return new FailResponse<NoteResourceResponse>($"Failed to write note to path {notePath} due to exception: {ex.Message}");
            }
        }

        private string GetFixedPrivateNotePath(string notePath)
        {
            string noteName = Path.GetFileName(notePath);
            string noteNameWithExtension = Path.ChangeExtension(noteName, AppConsts.NoteExtension);

            return Path.Combine(mTasksNotesDirectory, noteNameWithExtension);
        }
    }
}