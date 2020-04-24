import React, { Component } from 'react';
import FunctionalButton from '../ui-components/FunctionalButton'
import AutoCompleteTextField from '../ui-components/AutocompleteTextField'
import TaskerHttpRequester from '../../utils/TasksFunctions';
import TaskerApiUrls from '../../common/TaskerApiUrls';

export class WorkTaskViewer extends Component {

    constructor(props) {
        super(props);

        this.state = {
            tasks: [],
            groupsNames: [],
            isLoading: true,
            moveState: { isMoving: false, movingTaskId: null, destinationGroup: null },
            workTaskUrl: TaskerApiUrls.getWorkTaskUrl(),
            tasksGroupsUrl: TaskerApiUrls.getTasksGroupsUrl(),
        };
    }

    async componentDidMount() {
        const urlRequestParameter = window.location.pathname.split('/')[2];
        let tasksUrlRequest = this.state.workTaskUrl;

        if (urlRequestParameter !== "tasks")
            tasksUrlRequest += urlRequestParameter;

        const data = await TaskerHttpRequester.getHttpRequest(tasksUrlRequest);

        this.setState({
            tasks: data,
            groupsNames: await this.getGroupsNames(),
            isLoading: false
        });
    }

    renderWorkTasksTable(tasks) {
        return (
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Parent</th>
                        <th>Description</th>
                        <th>Status</th>
                        <th>Options</th>
                    </tr>
                </thead>
                <tbody>
                    {tasks.map(task =>
                        <tr key={task.taskId}>
                            <td>{task.taskId}</td>
                            <td>{task.groupName}</td>
                            <td>{task.description}</td>
                            <td>{task.status}</td>
                            <td>
                                <FunctionalButton
                                    onClickFunction={() => TaskerHttpRequester.postHttpRequest(
                                        this.state.workTaskUrl + task.taskId, this.createNewWorkTaskDescriptionObject())}
                                    buttonName="update"
                                />
                                <FunctionalButton
                                    onClickFunction={() => TaskerHttpRequester.deleteHttpRequest(this.state.workTaskUrl + task.taskId)}
                                    buttonName="delete"
                                />
                                <FunctionalButton
                                    onClickFunction={() => this.handleMove(task.taskId)}
                                    buttonName="move"
                                />
                                {this.viewAutoCompleteTextFieldForRelevantTaskId(task.taskId)}
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    handleMove(taskId) {
        if (!this.state.moveState.isMoving) {
            this.setMovingStateWithTaskId(taskId);
            return;
        }

        if (this.state.moveState.movingTaskId != taskId) {
            this.setMovingStateWithTaskId(taskId);
            return;
        }

        if (this.state.moveState.destinationGroup === null) {
            alert("destinationGroup is null");
            return;
        }

        TaskerHttpRequester.postHttpRequest(
            this.state.workTaskUrl + taskId, this.createNewWorkTaskParentGroupObject(this.state.moveState.destinationGroup));
    }

    setMovingStateWithTaskId(taskId) {
        const newMoveState = {
            isMoving: true,
            movingTaskId: taskId,
            destinationGroup: this.state.moveState.destinationGroup
        }

        this.setState({
            moveState: newMoveState
        });
    }

    setMovingStateWithDestinationGroup(destinationGroup) {
        let newMoveState = {
            isMoving: true,
            movingTaskId: this.state.moveState.movingTaskId,
            destinationGroup: destinationGroup
        }

        this.setState({
            moveState: newMoveState
        });
    }

    viewAutoCompleteTextFieldForRelevantTaskId(taskId) {
        return this.state.moveState.isMoving && this.state.moveState.movingTaskId === taskId
            ? <AutoCompleteTextField
                options={this.state.groupsNames}
                label="new group"
                action={this.setMovingStateWithDestinationGroup.bind(this)} />
            : null;
    }

    createNewWorkTaskDescriptionObject() {
        let taskDescription = window.prompt('Type new task description');
        return { description: taskDescription };
    }

    createNewWorkTaskParentGroupObject(groupName) {
        return { groupName: groupName };
    }

    addTask() {
        let taskDescription = window.prompt('Type task description');
        let groupName = window.prompt('Type group name');
        return {
            description: taskDescription,
            groupName: groupName
        };
    }

    async getGroupsNames() {
        const groups = await TaskerHttpRequester.getHttpRequest(this.state.tasksGroupsUrl);
        let groupsNames = [];
        groups.map(group => groupsNames.push(group.groupName));

        return groupsNames;
    }

    render() {
        let contents = this.state.isLoading
            ? <p><em>Loading...</em></p>
            : this.renderWorkTasksTable(this.state.tasks);

        return (
            <div>
                <h1>Work Tasks</h1>
                <p> </p>
                <FunctionalButton
                    onClickFunction={() => TaskerHttpRequester.putHttpRequest(
                        this.state.workTaskUrl, this.addTask())}
                    buttonName="Add Task"
                />
                <p> </p>
                {contents}
            </div>
        );
    }
}