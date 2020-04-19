import React, { Component } from 'react';
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
        const data = await TaskerHttpRequester.getHttpRequest(this.state.url + window.location.pathname.split('/')[2]);
        this.setState({ tasks: data, loading: false });
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
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderWorkTasksTable(this.state.tasks);

        return (
            <div>
                <h1>Work Tasks</h1>
                <p> </p>
                <button>Add Task</button>
                <p> </p>
                {contents}
            </div>
        );
    }
}