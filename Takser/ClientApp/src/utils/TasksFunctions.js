import React from 'react';

class TaskerHttpRequester extends React.Component {

    static deleteHttpRequest(httpRequest) {
        fetch(httpRequest,
            {
                method: 'DELETE'
            })
            .then(response => {
                if (!response.ok) {
                    alert("Response status code: " + response.status + "\n" +
                        "Error Message: " + response.statusText);
                    return;
                }

                return response.json();
            })
    };

    static postHttpRequest(httpRequest, jsonObject) {
        fetch(httpRequest,
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(jsonObject)
            })
            .then(response => {
                if (!response.ok) {
                    alert("Response status code: " + response.status + "\n" +
                        "Error Message: " + response.statusText);
                    return;
                }

                return response.json();
            })
    };
}

export default TaskerHttpRequester;