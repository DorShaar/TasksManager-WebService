import React from 'react'

export default function FunctionalButton(props) {
    const handleClick = function () { props.onClickFunction(); }
    return (
        <button onClick={handleClick}>
            {props.buttonName}
        </button>
    );
}