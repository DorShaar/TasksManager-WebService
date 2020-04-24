import React from 'react';
import TextField from '@material-ui/core/TextField';
import Autocomplete from '@material-ui/lab/Autocomplete';

export default function AutocompleteTextField(props) {
    const defaultProps = {
        options: props.options,
    };

    const [value, setValue] = React.useState(null);

    return (
        <div style={{ width: 300 }}>
            <Autocomplete
                {...defaultProps}
                id="group-text-field"
                value={value}
                onChange={(event, newValue) => {
                    setValue(newValue);
                    props.action(newValue);
                }}
                renderInput={(params) => <TextField {...params} label={props.label} margin="normal"/>}
            />
        </div>
    );
}