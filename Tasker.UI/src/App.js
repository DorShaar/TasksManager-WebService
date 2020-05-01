import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { TasksGroupViewer } from './components/viewers/TasksGroupViewer';
import { WorkTaskViewer } from './components/viewers/WorkTaskViewer';
import { NoteViewer } from './components/viewers/NoteViewer';
import TaskerCache from './components/domain/TaskerCache';
import TaskerUrls from './common/TaskerUrls';

export default class App extends Component {
    static displayName = App.name;

    render() {

        var taskerCache = new TaskerCache(
            TaskerUrls.getTasksGroupsApiUrl(), 
            TaskerUrls.getWorkTaskApiUrl(),
            TaskerUrls.getNotesApiUrl());

        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route exact path= {TaskerUrls.getGroupsViewerUrl()} render={() => <TasksGroupViewer cache={taskerCache} />} />
                <Route exact path= {TaskerUrls.getTasksViewerUrlByGroup()} render={() => <WorkTaskViewer cache={taskerCache}/>} />
                <Route exact path= {TaskerUrls.getTasksViewerUrl()} render={() => <WorkTaskViewer cache={taskerCache}/>} />
                <Route exact path= {TaskerUrls.getNoteViewerUrl()} render={() => <NoteViewer cache={taskerCache}/>} />
            </Layout>
        );
    }
}