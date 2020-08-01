import React from 'react';

export default class TaskerUrls extends React.Component {

    static getWorkTaskApiUrl() {
        // return 'http://tasker-api:8080/api/WorkTasks/';
        return 'http://tasker-api:31490/api/WorkTasks/';
    }

    static getTasksGroupsApiUrl() {
        // return 'http://tasker-api:8080/api/TasksGroups/';
        return 'http://tasker-api:31490/api/TasksGroups/';
    }

    static getNotesApiUrl() {
        // return 'http://tasker-api:8080/api/Notes/';
        return 'http://tasker-api:31490/api/Notes/';
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