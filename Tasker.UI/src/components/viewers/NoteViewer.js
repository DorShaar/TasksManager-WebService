import React, { Component } from 'react';
import CustomTreeItem from '../ui-components/CustomTreeItem';

export class NoteViewer extends Component {

    constructor(props) {
        super(props);

        this.state = {
            notes: {},
            noteContent: "Please choose note",
            cache: props.cache,
            loading: true
        };
    }

    async componentDidMount() {
        const data = await this.state.cache.getGeneralNotes();
        this.setState({ notes: data, loading: false });
    }

    async getNote(notePath) {
        const noteText = await this.state.cache.getNoteText(notePath);
        this.setState({ noteContent: noteText});
    }

    render() {
        const notesTree = this.state.loading 
        ? <p><em>Loading...</em></p> 
        : <CustomTreeItem treeData={this.state.notes} onClickEvent={this.getNote.bind(this)}/>;

        return (
            <div>
                <h1>Notes</h1>
                <p> </p>
                {notesTree}
                <h1> Content</h1>
                <p style={{ whiteSpace: "pre-wrap", }}> 
                    {this.state.noteContent}
                </p>
            </div>
        );
    }
}