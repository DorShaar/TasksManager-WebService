import React from 'react';

export default class TaskerApiUrls extends React.Component {

    static getWorkTaskUrl() {
        return 'https://localhost:5001/api/WorkTasks/';
    }

    static getTasksGroupsUrl() {
        return 'https://localhost:5001/api/TasksGroups/';
    }
}