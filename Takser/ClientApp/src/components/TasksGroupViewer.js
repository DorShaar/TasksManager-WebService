import React, { Component } from 'react';
import FunctionalButton from './FunctionalButton';

export class TasksGroupViewer extends Component {

    constructor(props) {
        super(props);
        this.state = { groups: [], loading: true };

        fetch('api/TasksGroups/Groups',
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
            .then(data => this.setState({ groups: data, loading: false }))
    }

    deleteGroup(id) {
        fetch('api/TasksGroups/' + id,
            {
                method: 'DELETE'
            })
            .then(response => {
                if (!response.ok) {
                    alert("Status Code: " + response.status + "\n" +
                        "Error Message: " + response.statusText);
                }

                return response.json();
            })
    }

    renderTasksGroupsTable(groups) {
        return (
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Status</th>
                        <th>Size</th>
                        <th>Options</th>
                    </tr>
                </thead>
                <tbody>
                    {groups.map(group =>
                        <tr key={group.groupId}>
                            <td>{group.groupId}</td>
                            <td>{group.groupName}</td>
                            <td>{group.isFinished}</td>
                            <td>{group.size}</td>
                            <td>
                                <FunctionalButton /*onClickFunction={displayFunction}*/ buttonName="view" />
                                <FunctionalButton /*onClickFunction={displayFunction}*/ buttonName="update" />
                                <FunctionalButton onClickFunction={() => this.deleteGroup(group.groupId)} buttonName="delete" />
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
            : this.renderTasksGroupsTable(this.state.groups);

        return (
            <div>
                <h1>Task Groups</h1>
                <p> </p>
                <button>Add Group</button>
                <button>View Open Groups</button>
                <p> </p>
                {contents}
            </div>
        );
    }
}