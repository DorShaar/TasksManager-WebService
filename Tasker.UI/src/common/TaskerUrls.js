import React from 'react';

export default class TaskerUrls extends React.Component {

    static getWorkTaskApiUrl() {
        return 'https://localhost:5001/api/WorkTasks/';
    }

    static getTasksGroupsApiUrl() {
        return 'https://localhost:5001/api/TasksGroups/';
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