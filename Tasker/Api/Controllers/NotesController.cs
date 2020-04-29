using AutoMapper;
using Logger.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskData.Contracts;

namespace Tasker.Api.Controllers
{
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private readonly INotesService mNotesService;
        private readonly IMapper mMapper;
        private readonly ILogger mLogger;

        public NotesController(INotesService notesService, IMapper mapper, ILogger logger)
        {
            mNotesService = notesService;
            mMapper = mapper;
            mLogger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<NoteResource>> ListGeneralNotesAsync()
        {
            mLogger.Log("Requesting general notes");

            IEnumerable<INote> notes = await mNotesService.ListAsync();

            IEnumerable<NoteResource> notesResources = mMapper
                .Map<IEnumerable<ITasksGroup>, IEnumerable<NoteResource>>(notes);

            mLogger.Log($"Found {notesResources.Count()} notes");

            return notesResources;
        }
    }
}