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
            <th>Date</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {groups.map(group =>
            <tr key={group.dateFormatted}>
              <td>{group.dateFormatted}</td>
              <td>{group.temperatureC}</td>
              <td>{group.temperatureF}</td>
              <td>{group.summary}</td>
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
