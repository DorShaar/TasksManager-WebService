using AutoMapper;
using TaskData.TasksGroups;
using TaskData.WorkTasks;
using Tasker.App.Resources;
using Tasker.App.Resources.Note;

namespace Tasker.App.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<ITasksGroup, TasksGroupResource>()
                .ForMember(dest => dest.GroupId,
                            opt => opt.MapFrom(source => source.ID))
                .ForMember(dest => dest.GroupName,
                            opt => opt.MapFrom(source => source.Name))
                .ForMember(dest => dest.Status,
                                opt => opt.MapFrom(source => source.IsFinished ? "Closed" : "Open"))
                .ForMember(dest => dest.Size,
                                opt => opt.MapFrom(source => source.Size));

            CreateMap<IWorkTask, WorkTaskResource>()
                .ForMember(dest => dest.TaskId,
                           option => option.MapFrom(source => source.ID))
                .ForMember(dest => dest.GroupName,
                           option => option.MapFrom(source => source.GroupName))
                .ForMember(dest => dest.Description,
                           option => option.MapFrom(source => source.Description))
                .ForMember(dest => dest.Status,
                           option => option.MapFrom(source => source.Status.ToString()));

            CreateMap<NoteResourceResponse, NoteResource>()
                .ForMember(dest => dest.IsNoteFound,
                           option => option.MapFrom(source => source.IsNoteFound))
                .ForMember(dest => dest.IsMoreThanOneNoteFound,
                           option => option.MapFrom(source => source.IsMoreThanOneNoteFound))
                .ForMember(dest => dest.PossibleNotes,
                           option => option.MapFrom(source => source.PossibleNotes))
                .ForMember(dest => dest.Extension,
                           option => option.MapFrom(source => source.Note.Extension))
                .ForMember(dest => dest.NotePath,
                           option => option.MapFrom(source => source.Note.NotePath))
                .ForMember(dest => dest.Text,
                           option => option.MapFrom(source => source.Note.Text));
        }
    }
}