import React, { Component } from 'react';
import FunctionalButton from '../ui-components/FunctionalButton';
import AutoCompleteTextField from '../ui-components/AutocompleteTextField';
import SwitchLabel from '../ui-components/SwitchLabel';

export class WorkTaskViewer extends Component {

    constructor(props) {
        super(props);

        this.state = {
            tasks: [],
            groupsNames: [],
            cache: props.cache,
            moveState: { isMoving: false, movingTaskId: null, destinationGroup: null },
            shouldDisplayAllGroups: false,
            isLoading: true,
        };
    }

    async componentDidMount() {
        let data;

        if (window.location.pathname.split('/')[2] === "tasks") {
            data = await this.state.cache.getTasks();
        }
        else {
            data = await this.state.cache.getTasks(window.location.pathname.split('/')[3]);
        }

        this.setState({
            tasks: data,
            groupsNames: await this.getGroupsNames(),
            isLoading: false
        });
    }

    async getGroupsNames() {
        const groups = await this.state.cache.getGroups();
        let groupsNames = [];
        groups.map(group => groupsNames.push(group.groupName));

        return groupsNames;
    }

    getWorkTasksTable(tasks) {
        const tasksToDisplay = !this.state.shouldDisplayAllGroups
            ? tasks.filter(group => group.status !== "Closed")
            : tasks;

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
                    {tasksToDisplay.map(task =>
                        <tr key={task.taskId}>
                            <td>{task.taskId}</td>
                            <td>{task.groupName}</td>
                            <td>{task.description}</td>
                            <td>{task.status}</td>
                            <td>
                                <FunctionalButton
                                    onClickFunction={() => this.state.cache.updateTask(
                                        task.taskId,
                                        this.createNewWorkTaskDescriptionObject()
                                    )}
                                    buttonName="update"
                                />
                                <FunctionalButton
                                    onClickFunction={() => this.state.cache.deleteTask(task.taskId)}
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

    async handleMove(taskId) {
        if (!this.state.moveState.isMoving) {
            this.setMovingStateWithTaskId(taskId);
            return;
        }

        if (this.state.moveState.movingTaskId !== taskId) {
            this.setMovingStateWithTaskId(taskId);
            return;
        }

        if (this.state.moveState.destinationGroup === null) {
            alert("destinationGroup is null");
            return;
        }

        await this.state.cache.moveTask(taskId, this.state.moveState.destinationGroup);
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
        return window.prompt('Type new task description');
    }

    getTasksPageView(workTasksTable) {
        return (
            <div>
                <p> </p>
                <FunctionalButton
                    onClickFunction={() => this.state.cache.addTask(this.addTask())}
                    buttonName="Add Task"
                />
                <p> </p>
                <SwitchLabel label="view all" action={this.toggleGroupsDisplay.bind(this)} />
                <p> </p>
                {workTasksTable}
            </div>
        );
    }

    addTask() {
        let taskDescription = window.prompt('Type task description');
        let groupName = window.prompt('Type group name');
        return {
            description: taskDescription,
            groupName: groupName
        };
    }

    toggleGroupsDisplay() {
        this.setState({ shouldDisplayAllGroups: !this.state.shouldDisplayAllGroups });
    }

    render() {
        let tasksPageView = this.state.isLoading
            ? <p><em>Loading...</em></p>
            : this.getTasksPageView(this.getWorkTasksTable(this.state.tasks));

        return (
            <div>
                <h1>Work Tasks</h1>
                {tasksPageView}
            </div>
        );
    }
}