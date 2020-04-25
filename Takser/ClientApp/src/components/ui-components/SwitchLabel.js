import React from 'react';
import FormGroup from '@material-ui/core/FormGroup';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Switch from '@material-ui/core/Switch';

export default function SwitchLabel(props) {
    const [state, setState] = React.useState({
        checkedA: false,
    });

    const handleChange = (event) => {
        setState({ ...state, [event.target.name]: event.target.checked });
        props.action();
    };

    return (
        <FormGroup row>
            <FormControlLabel
                style={{ color: "blue" }}
                control={<Switch checked={state.checkedA} onChange={handleChange} name="checkedA" />}
                label={props.label}
            />
        </FormGroup>
    );
}