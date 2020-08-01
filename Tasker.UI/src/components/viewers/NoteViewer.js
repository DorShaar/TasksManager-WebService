import React, { Component } from 'react';

export class NoteViewer extends Component {

    constructor(props) {
        super(props);

        this.state = {
            noteContent: "",
            cache: props.cache,
        };
    }

    async componentDidMount() {
        const noteId = window.location.pathname.split('/')[3];
        await this.getNote("note/" + noteId);
    }


    async getNote(notePath) {
        const noteText = await this.state.cache.getNoteText(notePath);
        this.setState({ noteContent: noteText});
    }

    render() {
        return (
            <div>
                <h1>Content</h1>
                <p style={{ whiteSpace: "pre-wrap", }}> 
                    {this.state.noteContent}
                </p>
            </div>
        );
    }
}