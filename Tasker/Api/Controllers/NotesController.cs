using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using TaskData.Notes;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.Api.Controllers
{
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly INoteService mNoteService;
        private readonly ILogger<NotesController> mLogger;

        public NotesController(INoteService noteService, ILogger<NotesController> logger)
        {
            mNoteService = noteService ?? throw new ArgumentNullException(nameof(noteService));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<NoteNode> GetGeneralNotesStructureAsync()
        {
            mLogger.LogDebug("Requesting for general notes structure");

            NoteNode notesStructure = await mNoteService.GetNotesStructure().ConfigureAwait(false);

            return notesStructure;
        }

        [HttpGet("{notePath}")]
        public async Task<IActionResult> GetGeneralNoteAsync(string notePath)
        {
            if (string.IsNullOrEmpty(notePath))
                return BadRequest("Note path is null or empty");

            notePath = GetFixedNotePath(notePath);

            mLogger.LogDebug($"Requesting text of general note {notePath}");

            try
            {
                IResponse<INote> result = await mNoteService.GetGeneralNote(notePath).ConfigureAwait(false);

                mLogger.LogDebug($"Get result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);

                mLogger.LogDebug($"Note text: {result.ResponseObject.Text}");
                return Ok(result.ResponseObject.Text);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Update operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("private/")]
        public async Task<NoteNode> GetPrivateNotesStructureAsync()
        {
            mLogger.LogDebug("Requesting for pirvate notes structure");

            NoteNode notesStructure = await mNoteService.GetNotesStructure().ConfigureAwait(false);

            return notesStructure;
        }

        [HttpGet("note/{noteIdentifier}")]
        public async Task<IActionResult> GetPrivateNoteAsync(string noteIdentifier)
        {
            if (string.IsNullOrEmpty(noteIdentifier))
                return BadRequest("Note is null or empty");

            noteIdentifier = GetFixedNotePath(noteIdentifier);

            mLogger.LogDebug($"Requesting text of private note {noteIdentifier}");

            try
            {
                IResponse<INote> result = await mNoteService.GetTaskNote(noteIdentifier).ConfigureAwait(false);

                mLogger.LogDebug($"Get result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);

                mLogger.LogDebug($"Note text: {result.ResponseObject.Text}");
                return Ok(result.ResponseObject.Text);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Update operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private string GetFixedNotePath(string notePath)
        {
            return notePath.Replace('*', Path.DirectorySeparatorChar);
        }
    }
}