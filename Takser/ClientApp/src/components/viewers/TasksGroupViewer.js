import React, { Component } from 'react';
import FunctionalButton from '../ui-components/FunctionalButton'

export class TasksGroupViewer extends Component {

    constructor(props) {
        super(props);

        this.state = {
            groups: [],
            loading: true,
            cache: props.cache
        };
    }

    async componentDidMount() {
        const data = await this.state.cache.getGroups();
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
                                    onClickFunction={() => this.viewGroupTasks(group.groupId)}
                                    buttonName="view"
                                />
                                <FunctionalButton
                                    onClickFunction={() => this.state.cache.updateGroupName(
                                        group.groupId,
                                        this.createNewGroupName()
                                    )}
                                    buttonName="update"
                                />
                                <FunctionalButton
                                    onClickFunction={() => this.state.cache.deleteGroup(group.groupId)}
                                    buttonName="delete"
                                />
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    viewGroupTasks(groupId) {
        window.location.pathname += "/" + groupId;
    }

    createNewGroupName() {
        return window.prompt('Type new group name');
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderTasksGroupsTable(this.state.groups);

        return (
            <div>
                <h1>Task Groups</h1>
                <p> </p>
                <FunctionalButton
                    onClickFunction={() => this.state.cache.addGroup(this.createNewGroupName())}
                    buttonName="Add Group"
                />
                <p> </p>
                {contents}
            </div>
        );
    }
}