import React, { Component } from 'react';
import FunctionalButton from './FunctionalButton';
import TaskerHttpRequester from '../utils/TasksFunctions';

export class TasksGroupViewer extends Component {

    constructor(props) {
        super(props);
        this.state = {
            groups: [],
            loading: true,
            url: 'api/TasksGroups/'
        };
    }

    async componentDidMount() {
        const data = await TaskerHttpRequester.getHttpRequest(this.state.url);
        this.setState({ groups: data, loading: false });
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
                            <td>{group.status}</td>
                            <td>{group.size}</td>
                            <td>
                                <FunctionalButton
                                    onClickFunction={() => this.ViewGroupTasks(group.groupId)}
                                    buttonName="view"
                                />
                                <FunctionalButton
                                    onClickFunction={() => TaskerHttpRequester.postHttpRequest(
                                            this.state.url + group.groupId, this.createNewGroupNameObject())}
                                    buttonName="update"
                                />
                                <FunctionalButton
                                    onClickFunction={() => TaskerHttpRequester.deleteHttpRequest(this.state.url + group.groupId)}
                                    buttonName="delete"
                                />
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    ViewGroupTasks(groupId) {
        window.location.pathname += "/" + groupId;
    }

    createNewGroupNameObject() {
        let newGroupName = window.prompt('Type new group name');
        return { GroupName: newGroupName };
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
                <p> </p>
                {contents}
            </div>
        );
    }
}