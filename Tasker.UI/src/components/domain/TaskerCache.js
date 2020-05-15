import TaskerHttpRequester from '../../utils/TasksFunctions';

export default class TaskerCache {

    constructor(tasksGroupsUrl, workTasksUrl, notesUrl) {
        this.tasksGroupsUrl = tasksGroupsUrl;
        this.workTasksUrl = workTasksUrl;
        this.notesUrl = notesUrl;

        this.groupsCache = {
            isSync: false,
            groups: []
        };
    };

    // Groups methods.

    async getGroups() {
        window.console.log("Requesting all groups");

        if (!this.groupsCache.isSync) {
            await this.syncGroups();
        }

        return this.groupsCache.groups;
    }

    async addGroup(newGroupName) {
        window.console.log("Adding group " + newGroupName);
        await TaskerHttpRequester.putHttpRequest(this.tasksGroupsUrl, { GroupName: newGroupName });
        this.groupsCache.isSync = false;
    }

    async updateGroupName(groupId, newGroupName) {
        window.console.log("Adding group id" + groupId + " with new group name " + newGroupName);
        await TaskerHttpRequester.postHttpRequest(this.tasksGroupsUrl + groupId, { GroupName: newGroupName });
        this.groupsCache.isSync = false;
    }

    async deleteGroup(groupId) {
        if (window.confirm("Are you sure you want to delete group " + groupId)) {
            window.console.log("Deleting group id" + groupId);
            await TaskerHttpRequester.deleteHttpRequest(this.tasksGroupsUrl + groupId);
            this.groupsCache.isSync = false;
        }
    }

    async syncGroups() {
        this.groupsCache.groups = await TaskerHttpRequester.getHttpRequest(this.tasksGroupsUrl);
        this.groupsCache.isSync = true;
    }

    // Tasks methods.

    async getTasks(groupId) {

        if (groupId === undefined || groupId === null) {
            window.console.log("Getting all work tasks");
            return await TaskerHttpRequester.getHttpRequest(this.workTasksUrl);
        }
            
        window.console.log("Getting all work tasks of group " + groupId);
        return await TaskerHttpRequester.getHttpRequest(this.workTasksUrl + groupId);
    }

    async addTask(newTaskObject) {
        window.console.log("Adding work task");
        await TaskerHttpRequester.putHttpRequest(this.workTasksUrl, newTaskObject);
    }

    async updateTask(taskId, newDescription) {
        window.console.log("Updating work task " + taskId + " with new description " + newDescription);
        await TaskerHttpRequester.postHttpRequest(this.workTasksUrl + taskId, { description: newDescription });
    }

    async moveTask(taskId, destinationGroup) {
        window.console.log("Moving work task " + taskId + " to new group " + destinationGroup);
        await TaskerHttpRequester.postHttpRequest(this.workTasksUrl + taskId, { groupName: destinationGroup });
    }

    async deleteTask(taskId) {
        if (window.confirm("Are you sure you want to delete task " + taskId)) {
            window.console.log("Deleting work task " + taskId);
            await TaskerHttpRequester.deleteHttpRequest(this.workTasksUrl + taskId);
        }
    }

    // Notes methods.

    async getGeneralNotes() {
        window.console.log("Getting all general notes");
        return await TaskerHttpRequester.getHttpRequest(this.notesUrl);
    }

    async getNoteText(notePath) {
        window.console.log("Getting note of path " + notePath);
        return await TaskerHttpRequester.getHttpTextRequest(this.notesUrl + notePath);
    }
}