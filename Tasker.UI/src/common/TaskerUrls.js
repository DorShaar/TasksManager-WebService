export const ApiPrefix = "https://localhost:5001/api"; // http://tasker-api:31490/api

export const TaskerUrls = {
    WorkTaskApi:     `${ApiPrefix}/WorkTasks/`,
    TasksGroupsApi:  `${ApiPrefix}/TasksGroups/`,
    NotesApi:        `${ApiPrefix}/Notes/`,
};

export const TaskerRoutes = {

    GroupsViewer:        '/tasker/groups',
    TasksViewerByGroup:  '/tasker/group/:groupId',
    TasksViewer:         '/tasker/tasks',
    NotesViewer:         '/tasker/notes',
    NoteViewer:          '/tasker/notes/:noteId',
    LoginViewer:         '/tasker/login',
};