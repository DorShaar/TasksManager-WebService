import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { TasksGroupViewer } from './components/TasksGroupViewer';
import { WorkTaskViewer } from './components/WorkTaskViewer';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route exact path='/tasks-groups' component={TasksGroupViewer} />
                <Route path='/tasks-groups/:groupId' component={WorkTaskViewer} />
            </Layout>
        );
    }
}