import React from 'react';

function alertError(response) {
    alert("Response status code: " + response.status + "\n" +
        "Error Message: " + response.statusText);
}

export default class TaskerHttpRequester extends React.Component {

    static async getHttpRequest(url) {
        const response = await fetch(url, {
            headers: {
                "Content-Type": "application/json",
            },
            credentials: 'include'
        });

        if (!response.ok) {
            alertError(response);
            return;
        }

        return await response.json();
    }

    static async getHttpTextRequest(url) {
        const response = await fetch(url, {
            headers: {
                "Content-Type": "text/plain",
            },
            credentials: 'include'
        });

        if (!response.ok) {
            alertError(response);
            return;
        }

        return await response.text();
    }

    static async deleteHttpRequest(url) {
        const response = await fetch(url,
            {
                method: 'DELETE'
            });

        if (!response.ok) {
            alertError(response);
            return;
        }

        return await response.json();
    };

    static async postHttpRequest(url, jsonObject) {
        const response = await fetch(url,
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(jsonObject)
            });

        if (!response.ok) {
            alertError(response);
            return;
        }

        return await response.json();
    };

    static async putHttpRequest(url, jsonObject) {
        const response = await fetch(url,
            {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(jsonObject)
            });

        if (!response.ok) {
            alertError(response);
            return;
        }

        return await response.json();
    };
}