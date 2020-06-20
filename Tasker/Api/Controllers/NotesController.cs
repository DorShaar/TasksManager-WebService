using AutoMapper;
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
        private readonly IMapper mMapper;
        private readonly ILogger mLogger;

        public NotesController(INoteService noteService, IMapper mapper, ILogger logger)
        {
            mNoteService = noteService;
            mMapper = mapper;
            mLogger = logger;
        }

        [HttpGet]
        public async Task<NoteNode> GetGeneralNotesStructureAsync()
        {
            mLogger.Log("Requesting for general notes structure");

            NoteNode notesStructure = await mNoteService.GetNotesStructure();

            return notesStructure;
        }

        [HttpGet("{notePath}")]
        public async Task<IActionResult> GetGeneralNoteAsync(string notePath)
        {
            if (notePath == null)
                return BadRequest("Note path is null");

            notePath = notePath.Replace('-', Path.DirectorySeparatorChar);

            mLogger.Log($"Requesting text of general note {notePath}");

            try
            {
                IResponse<INote> result = await mNoteService.GetNote(notePath);

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
    }
}