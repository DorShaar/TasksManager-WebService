import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { LoginPage } from './components/LoginPage';
import { TasksGroupViewer } from './components/TasksGroupViewer';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Route exact path='/' component={LoginPage} />
                <Route path='/tasks-groups' component={TasksGroupViewer} />
            </Layout>
        );
    }
}
