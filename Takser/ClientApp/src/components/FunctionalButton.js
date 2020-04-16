import React from 'react'

function FunctionalButton(props) {
    const handleClick = () => props.onClickFunction();
    return (
        <button onClick={handleClick}>
            {props.buttonName}
        </button>
    );
}

export default FunctionalButton