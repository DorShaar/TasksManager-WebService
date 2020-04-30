using AutoMapper;
using Logger.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tasker.App.Services;
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
        public async Task<NoteNode> ListGeneralNotesAsync()
        {
            mLogger.Log("Requesting general notes");

            NoteNode notesResource = await mNoteService.GetNotesStructure();

            mLogger.Log($"Found general notes");

            return notesResource;
        }
    }
}