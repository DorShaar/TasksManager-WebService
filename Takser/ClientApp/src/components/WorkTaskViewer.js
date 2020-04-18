import React, { Component } from 'react';
import FunctionalButton from './FunctionalButton';
import TaskerHttpRequester from '../utils/TasksFunctions'

export class WorkTaskViewer extends Component {

    constructor(props) {
        super(props);
        this.state = {
            tasks: [],
            loading: true,
            url: 'api/TasksGroups/1005'
        };

        fetch(this.state.url,
            {
                headers: { "Content-Type": "application/json" },
                credentials: 'include'
            })
            .then(response => {
                if (!response.ok) {
                    throw response;
                }

                return response.json();
            })
            .then(data => this.setState({ tasks: data, loading: false }))
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