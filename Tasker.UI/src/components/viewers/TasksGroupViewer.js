import React, { Component } from 'react';
import FunctionalButton from '../ui-components/FunctionalButton';
import SwitchLabel from '../ui-components/SwitchLabel';
import TaskerUrls from '../../common/TaskerUrls';

export class TasksGroupViewer extends Component {

    constructor(props) {
        super(props);

        this.state = {
            groups: [],
            cache: props.cache,
            loading: true,
            shouldDisplayAllGroups: false 
        };
    }

    async componentDidMount() {
        const data = await this.state.cache.getGroups();
        this.setState({ groups: data, loading: false });
    }

    getTasksGroupsTable(groups) {
        const groupsToDisplay = !this.state.shouldDisplayAllGroups
            ? groups.filter(group => group.status !== "Closed")
            : groups;

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
                    {groupsToDisplay.map(group =>
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
        const newUrlLocation = TaskerUrls.getTasksViewerUrlByGroup().replace(":groupId", groupId);
        window.location.pathname = newUrlLocation;
    }

    getGroupsPageView(tasksGroupsTable) {
        return (
            <div>
                <p> </p>
                <FunctionalButton
                    onClickFunction={() => this.state.cache.addGroup(this.createNewGroupName())}
                    buttonName="Add Group"
                />
                <p> </p>
                <SwitchLabel label="view all" action={this.toggleGroupsDisplay.bind(this)} />
                <p> </p>
                {tasksGroupsTable}
            </div>
        );
    }

    toggleGroupsDisplay() {
        this.setState({ shouldDisplayAllGroups: !this.state.shouldDisplayAllGroups });
    }

    createNewGroupName() {
        return window.prompt('Type new group name');
    }

    render() {
        const pageView = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.getGroupsPageView(this.getTasksGroupsTable(this.state.groups));

        return (
            <div>
                <h1>Task Groups</h1>
                {pageView}
            </div>
        );
    }
}