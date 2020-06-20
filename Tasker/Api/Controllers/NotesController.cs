using Logger.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using TaskData.Contracts;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Models;

namespace Tasker.Api.Controllers
{
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly INoteService mNoteService;
        private readonly ILogger mLogger;

        public NotesController(INoteService noteService, ILogger logger)
        {
            mNoteService = noteService;
            mLogger = logger;
        }

        [HttpGet]
        public async Task<NoteNode> GetGeneralNotesStructureAsync()
        {
            mLogger.Log("Requesting for general notes structure");

            NoteNode notesStructure = await mNoteService.GetNotesStructure().ConfigureAwait(false);

            return notesStructure;
        }

        [HttpGet("{notePath}")]
        public async Task<IActionResult> GetGeneralNoteAsync(string notePath)
        {
            if (string.IsNullOrEmpty(notePath))
                return BadRequest("Note path is null or empty");

            notePath = GetFixedNotePath(notePath);

            mLogger.Log($"Requesting text of general note {notePath}");

            try
            {
                IResponse<INote> result = await mNoteService.GetGeneralNote(notePath).ConfigureAwait(false);

                mLogger.Log($"Get result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);

                mLogger.Log($"Note text: {result.ResponseObject.Text}");
                return Ok(result.ResponseObject.Text);
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Update operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("note/{noteIdentifier}")]
        public async Task<IActionResult> GetPrivateNoteAsync(string noteIdentifier)
        {
            if (string.IsNullOrEmpty(noteIdentifier))
                return BadRequest("Note is null or empty");

            noteIdentifier = GetFixedNotePath(noteIdentifier);

            mLogger.Log($"Requesting text of private note {noteIdentifier}");

            try
            {
                IResponse<INote> result = await mNoteService.GetTaskNote(noteIdentifier).ConfigureAwait(false);

                mLogger.Log($"Get result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);

                mLogger.Log($"Note text: {result.ResponseObject.Text}");
                return Ok(result.ResponseObject.Text);
            }
            catch (Exception ex)
            {
                mLogger.LogError($"Update operation failed with error", ex);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private string GetFixedNotePath(string notePath)
        {
            return notePath.Replace('*', Path.DirectorySeparatorChar);
        }
    }
}