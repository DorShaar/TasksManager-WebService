import React, { Component } from 'react';
import FunctionalButton from '../ui-components/FunctionalButton';

export class NoteViewer extends Component {

    constructor(props) {
        super(props);

        this.state = {
            loading: false
        };
    }

    async componentDidMount() {
    }

    renderTasksGroupsTable(noteContent) {
        return (
            <div>
                {noteContent}
            </div>
        );
    }

    getNoteContent() {
        return "abcd\nefg\nhijklmnop";
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderTasksGroupsTable(this.getNoteContent());

        return (
            <div>
                <h1>Notes</h1>
                <p> </p>
                <FunctionalButton
                    //onClickFunction={() => this.state.cache.addGroup(this.createNewGroupName())}
                    buttonName="Edit"
                />
                <p> </p>
                {contents}
            </div>
        );
    }
}