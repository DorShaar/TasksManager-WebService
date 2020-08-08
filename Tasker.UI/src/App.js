import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { TasksGroupViewer } from './components/viewers/TasksGroupViewer';
import { WorkTaskViewer } from './components/viewers/WorkTaskViewer';
import { NotesViewer } from './components/viewers/NotesViewer';
import { NoteViewer } from './components/viewers/NoteViewer';
import { ApplicationPaths, LoginActions } from './components/api-authorization/ApiAuthorizationConstants';
import { Login } from './components/api-authorization/Login';
import { LoginMenu } from './components/api-authorization/LoginMenu';
import { TaskerUrls, TaskerRoutes } from './common/TaskerUrls';
import TaskerCache from './components/domain/TaskerCache';

export default class App extends Component {
    static displayName = App.name;

    render() {
        var taskerCache = new TaskerCache(
            TaskerUrls.TasksGroupsApi,
            TaskerUrls.WorkTaskApi,
            TaskerUrls.NotesApi);

        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route exact path={TaskerRoutes.GroupsViewer} render={() => <TasksGroupViewer cache={taskerCache} />} />
                <Route exact path={TaskerRoutes.TasksViewerByGroup} render={() => <WorkTaskViewer cache={taskerCache}/>} />
                <Route exact path={TaskerRoutes.TasksViewer} render={() => <WorkTaskViewer cache={taskerCache}/>} />
                <Route exact path={TaskerRoutes.NotesViewer} render={() => <NotesViewer cache={taskerCache}/>} />
                <Route exact path={TaskerRoutes.NoteViewer} render={() => <NoteViewer cache={taskerCache}/>} />
                <Route exact path={TaskerRoutes.LoginViewer} render={() => <LoginMenu/>} />
                <Route path= {ApplicationPaths.Login} render={() => <Login action={LoginActions.Login}/>} />
            </Layout>
        );
    }
}