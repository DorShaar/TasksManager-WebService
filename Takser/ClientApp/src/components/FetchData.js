import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor (props) {
    super(props);
       this.state = { groups: [], loading: true };
       fetch('api/TasksGroups/Groups', {
            headers: { "Content-Type": "application/json" },
            credentials: 'include'
       })
            .then(response => {
                 if (!response.ok) {
                      throw response;
                 }
                 return response.json();
            })
            .then(data => {
                 this.setState({ groups: data, loading: false });
            });
  }

  static renderTasksGroupsTable (groups) {
    return (
      <table className='table table-striped'>
        <thead>
          <tr>
            <th>Id</th>
            <th>Name</th>
            <th>IsClosed</th>
            <th>Size</th>
          </tr>
        </thead>
        <tbody>
          {groups.map(group =>
            <tr key={group.groupId}>
              <td>{group.groupId}</td>
              <td>{group.groupName}</td>
              <td>{group.isFinished}</td>
              <td>{group.size}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render () {
    let contents = this.state.loading
         ? <p><em>Loading...</em></p>
         : FetchData.renderTasksGroupsTable(this.state.groups);

    return (
      <div>
        <h1>Task Groups</h1>
        <p> </p>
        {contents}
      </div>
    );
  }
}
