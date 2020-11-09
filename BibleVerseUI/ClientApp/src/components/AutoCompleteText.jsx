import React from 'react';
import '../css/AutoCompleteText.css';


const AutoCompleteText = props => {

    function renderItems() {
        console.log(props.items)
        return props.items != [] ? props.items.map(item => <li key={item.UserName}>{item.UserName}</li>) : <li>"Search Failed"</li>
    }

    return (
        <div>
            <ul>
                {renderItems()}
            </ul>
        </div>
        )
}

export default AutoCompleteText;