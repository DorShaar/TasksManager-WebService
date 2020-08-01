import React from 'react';

export default class TaskerUrls extends React.Component {

    static getWorkTaskApiUrl() {
        return 'https://localhost:5001/api/WorkTasks/';
        // return 'http://tasker-api:31490/api/WorkTasks/';
    }

    static getTasksGroupsApiUrl() {
        return 'https://localhost:5001/api/TasksGroups/';
        // return 'http://tasker-api:31490/api/TasksGroups/';
    }

    static getNotesApiUrl() {
        return 'https://localhost:5001/api/Notes/';
        // return 'http://tasker-api:31490/api/Notes/';
    }

    static getGroupsViewerUrl() {
        return '/tasker/groups';
    }

    static getTasksViewerUrlByGroup() {
        return '/tasker/group/:groupId';
    }

    static getTasksViewerUrl() {
        return '/tasker/tasks';
    }

    static getNotesViewerUrl() {
        return '/tasker/notes';
    }

    static getNoteViewerUrl() {
        return '/tasker/notes/:noteId';
    }
}