import React, { useState } from 'react';
import { Form } from 'react-bootstrap';
import AutoCompleteText from './AutoCompleteText';
import BBVAPI from '../middleware/BBVAPI';

class SearchBar extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            searchBody: '',
            items: []
        }
    }

    setPropVal(prop, val) {
        val = val.trim();

        this.setState({
            [prop]: val
        });

        if (val.length > 2) {
            this.query(this.state.searchBody);
        }
    }

    async query(val) {
        
        var result = await BBVAPI.query('ALL', val);

        await this.setState({ items: result });
        console.log(this.state.items);
    }

    render(){
        return (
            <Form className="AutoCompleteText">
                <Form.Control type="text" className="nav-search" placeholder="Search..." style={{width: "100%"}} value={this.state.searchBody ? this.state.searchBody : ''} onChange={(val) => this.setPropVal("searchBody", val.target.value)}/>
                <AutoCompleteText items={this.state.items} />
            </Form>
        )
    }
}

export default SearchBar;