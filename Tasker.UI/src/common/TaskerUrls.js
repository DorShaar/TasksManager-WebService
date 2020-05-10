import React from 'react';

export default class TaskerUrls extends React.Component {

    static getWorkTaskApiUrl() {
        return 'http://localhost:8081/api/WorkTasks/';
    }

    static getTasksGroupsApiUrl() {
        return 'http://localhost:8081/api/TasksGroups/';
    }

    static getNotesApiUrl() {
        return 'http://localhost:8081/api/Notes/';
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

    static getNoteViewerUrl() {
        return '/tasker/notes';
    }
}