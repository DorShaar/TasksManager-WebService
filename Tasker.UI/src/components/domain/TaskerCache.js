import TaskerHttpRequester from '../../utils/TasksFunctions';

export default class TaskerCache {

    constructor(tasksGroupsUrl, workTasksUrl) {
        this.tasksGroupsUrl = tasksGroupsUrl;
        this.workTasksUrl = workTasksUrl;

        this.groupsCache = {
            isSync: false,
            groups: []
        };
    };

    // Groups methods.

    async getGroups() {
        if (!this.groupsCache.isSync) {
            await this.syncGroups();
        }

        return this.groupsCache.groups;
    }

    async addGroup(newGroupName) {
        await TaskerHttpRequester.putHttpRequest(this.tasksGroupsUrl, { GroupName: newGroupName })
        this.groupsCache.isSync = false;
    }

    async updateGroupName(groupId, newGroupName) {
        await TaskerHttpRequester.postHttpRequest(this.tasksGroupsUrl + groupId, { GroupName: newGroupName });
        this.groupsCache.isSync = false;
    }

    async deleteGroup(groupId) {
        if (window.confirm("Are you sure you want to delete group " + groupId)) {
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

        if (groupId === undefined || groupId === null)
            return await TaskerHttpRequester.getHttpRequest(this.workTasksUrl);

        return await TaskerHttpRequester.getHttpRequest(this.workTasksUrl + groupId);
    }

    async addTask(newTaskObject) {
        await TaskerHttpRequester.putHttpRequest(this.workTasksUrl, newTaskObject);
    }

    async updateTask(taskId, newDescription) {
        await TaskerHttpRequester.postHttpRequest(this.workTasksUrl + taskId, { description: newDescription });
    }

    async moveTask(taskId, destinationGroup) {
        await TaskerHttpRequester.postHttpRequest(this.workTasksUrl + taskId, { groupName: destinationGroup });
    }

    async deleteTask(taskId) {
        if (window.confirm("Are you sure you want to delete task " + taskId)) {
            await TaskerHttpRequester.deleteHttpRequest(this.workTasksUrl + taskId);
        }
    }
}