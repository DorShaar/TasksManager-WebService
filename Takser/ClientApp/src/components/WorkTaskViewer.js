import React, { Component } from 'react';
import FunctionalButton from './FunctionalButton';
import TaskerHttpRequester from '../utils/TasksFunctions';

export class WorkTaskViewer extends Component {

    constructor(props) {
        super(props);
        this.state = {
            tasks: [],
            loading: true,
            url: 'api/TasksGroups/'
        };
    }

    async componentDidMount() {
        const newUrl = this.state.url + window.location.pathname.split('/')[2];
        const data = await TaskerHttpRequester.getHttpRequest(newUrl);

        this.setState({
            tasks: data,
            loading: false,
            url: newUrl
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
                                        this.state.url + task.taskId, this.createNewWorkTaskDescriptionObject())}
                                    buttonName="update"
                                />
                                <FunctionalButton
                                    onClickFunction={() => TaskerHttpRequester.deleteHttpRequest(this.state.url + task.taskId)}
                                    buttonName="delete"
                                />
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    createNewWorkTaskDescriptionObject() {
        let taskDescription = window.prompt('Type new task description');
        return { TaskDescription: taskDescription};
    }

    //addTask() {
    //    let taskDescription = window.prompt('Type task description');
    //    let groupName = window.prompt('Type group name');
    //    return {
    //        TaskDescription: taskDescription,
    //        groupName: groupName
    //    };
    //}

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderWorkTasksTable(this.state.tasks);

        return (
            <div>
                <h1>Work Tasks</h1>
                <p> </p>
                <FunctionalButton
                    onClickFunction={() => TaskerHttpRequester.postHttpRequest(
                        this.state.url, this.addTask())}
                    buttonName="Add Task"
                />
                <p> </p>
                {contents}
            </div>
        );
    }
}