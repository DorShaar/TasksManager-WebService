using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskData.WorkTasks;
using Tasker.App.Resources.Note;
using Tasker.App.Services;
using Tasker.Domain.Communication;
using Tasker.Domain.Extensions;
using Tasker.Domain.Models;
using Tasker.Infra.Consts;

namespace Tasker.Api.Controllers
{
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly INoteService mNoteService;
        private readonly IWorkTaskService mWorkTaskService;
        private readonly IMapper mMapper;
        private readonly ILogger<NotesController> mLogger;

        public NotesController(INoteService noteService,
            IWorkTaskService workTaskService,
            IMapper mapper,
            ILogger<NotesController> logger)
        {
            mNoteService = noteService ?? throw new ArgumentNullException(nameof(noteService));
            mWorkTaskService = workTaskService ?? throw new ArgumentNullException(nameof(workTaskService));
            mMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            mLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<NoteNode> GetGeneralNotesStructureAsync()
        {
            mLogger.LogDebug("Requesting for general notes structure");

            return await mNoteService.GetGeneralNotesStructure().ConfigureAwait(false);
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
                IResponse<NoteResourceResponse> result = await mNoteService.GetGeneralNote(notePath).ConfigureAwait(false);

                mLogger.LogDebug($"Get result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);

                NoteResource noteResource = mMapper.Map<NoteResourceResponse, NoteResource>(result.ResponseObject);

                if (noteResource.IsMoreThanOneNoteFound)
                    mLogger.LogDebug($"Found more than one note for {notePath}");
                else
                    mLogger.LogDebug($"Found note at path {notePath}");

                return Ok(noteResource);
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

            return await mNoteService.GetNotesStructure().ConfigureAwait(false);
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
                IResponse<NoteResourceResponse> result = await mNoteService.GetTaskNote(noteIdentifier).ConfigureAwait(false);

                mLogger.LogDebug($"Get result {(result.IsSuccess ? "succeeded" : "failed")}");

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);

                NoteResource noteResource = mMapper.Map<NoteResourceResponse, NoteResource>(result.ResponseObject);

                if (noteResource.IsMoreThanOneNoteFound)
                    mLogger.LogDebug($"Found more than one note for {noteIdentifier}");
                else
                    mLogger.LogDebug($"Found note for identifier {noteIdentifier}");

                return Ok(noteResource);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Update operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutNoteAsync([FromBody] NoteResource noteResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.GetErrorMessages());

            if (noteResource == null)
                return BadRequest($"Parameter {nameof(noteResource)} is null");

            IEnumerable<IWorkTask> workTasks =
                await mWorkTaskService.FindWorkTasksByConditionAsync(task => task.ID == noteResource.NotePath)
                .ConfigureAwait(false);

            if (!workTasks.Any())
                return StatusCode(StatusCodes.Status405MethodNotAllowed, $"Could not find work task {noteResource.NotePath}");

            mLogger.LogDebug($"Requesting putting new note {noteResource.NotePath}");

            IWorkTask firstWorkTask = workTasks.First();

            string noteName = $"{firstWorkTask.ID}-{firstWorkTask.Description}{AppConsts.NoteExtension}";

            try
            {
                IResponse<NoteResourceResponse> result =
                    await mNoteService.CreatePrivateNote(noteName, noteResource.Text).ConfigureAwait(false);

                NoteResource noteResourceResult = mMapper.Map<NoteResourceResponse, NoteResource>(result.ResponseObject);

                if (!result.IsSuccess)
                    return StatusCode(StatusCodes.Status405MethodNotAllowed, result.Message);

                return Ok(noteResourceResult);
            }
            catch (Exception ex)
            {
                mLogger.LogError(ex, "Put operation failed with error");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private static string GetFixedNotePath(string notePath)
        {
            return notePath.Replace('*', Path.DirectorySeparatorChar);
        }
    }
}