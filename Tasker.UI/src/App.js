import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { TasksGroupViewer } from './components/viewers/TasksGroupViewer';
import { WorkTaskViewer } from './components/viewers/WorkTaskViewer';
import { NoteViewer } from './components/viewers/NoteViewer';
import TaskerCache from './components/domain/TaskerCache';
import TaskerApiUrls from './common/TaskerApiUrls';

export default class App extends Component {
    static displayName = App.name;

    render() {

        var taskerCache = new TaskerCache(TaskerApiUrls.getTasksGroupsUrl(), TaskerApiUrls.getWorkTaskUrl());

        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route exact path='/tasker/groups' render={() => <TasksGroupViewer cache={taskerCache} />} />
                <Route exact path='/tasker/group/:groupId' render={() => <WorkTaskViewer cache={taskerCache} view= "group"/>} />
                <Route exact path='/tasker/tasks' render={() => <WorkTaskViewer cache={taskerCache} view="all"/>} />
                <Route exact path='/tasker/notes' render={() => <NoteViewer />} />
            </Layout>
        );
    }
}