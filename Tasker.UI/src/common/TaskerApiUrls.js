import React from 'react';

export default class TaskerApiUrls extends React.Component {

    static getWorkTaskUrl() {
        return 'api/WorkTasks/';
    }

    static getTasksGroupsUrl() {
        return 'api/TasksGroups/';
    }
}