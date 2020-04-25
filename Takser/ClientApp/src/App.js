import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { TasksGroupViewer } from './components/viewers/TasksGroupViewer';
import { WorkTaskViewer } from './components/viewers/WorkTaskViewer';
import TaskerCache from './components/domain/TaskerCache';
import TaskerApiUrls from './common/TaskerApiUrls';

export default class App extends Component {
    static displayName = App.name;

    render() {

        var taskerCache = new TaskerCache(TaskerApiUrls.getTasksGroupsUrl(), TaskerApiUrls.getWorkTaskUrl());

        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route exact path='/tasks-groups' render={() => <TasksGroupViewer cache={taskerCache} />} />
                <Route exact path='/tasks-groups/:groupIdOrTasks' render={() => <WorkTaskViewer cache={taskerCache} />} />
            </Layout>
        );
    }
}