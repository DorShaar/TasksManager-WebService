import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>Hello, Welcome to Tasker!</h1>
        <p>Here you can manage your tasks and notes</p>
      </div>
    );
  }
}
