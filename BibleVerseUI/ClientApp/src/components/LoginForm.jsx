﻿import React from 'react';
import { Button, Form, FormGroup, Label, Input, FormText } from 'reactstrap';

class LoginForm extends React.Component {
    static displayName = LoginForm.name;

    constructor(props) {
        super(props);

        this.state = {
            email: '',
            password: '',
            btnDisabled: false
        };
    }

    setPropVal(prop, val) {
        //val = val.trim();

        this.setState({
            [prop]: val
        });
    }


    async login() {
        window.console.log("clicked");

        this.setState({ btnDisabled: true });

        try {

            let res = await fetch("https://localhost:5001/api/Login/", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: this.state.email,
                    password: this.state.password
                })
            });

            let result = await res.json();

            if (result && result.success) {
            /*Do  something with response*/
                alert(result);
            } else if (result && result.success === false) {
                console.log("Nope");
                alert(result.msg);
            }

        } catch (exception) {
            console.log(exception);
        }

    }

    render() {
        return (
            <Form>
                <FormGroup>
                        <Label for="email">Email</Label>
                    <Input type="email" name="email" id="email" placeholder="Email" value={this.state.email ? this.state.email : ''} onChange={(val) => this.setPropVal("email", val.target.value)}/>
                </FormGroup>
                <FormGroup>
                        <Label for="password">Password</Label>
                    <Input type="password" name="password" id="password" placeholder="Password" value={this.state.password ? this.state.password : ''} onChange={(val) => this.setPropVal("password", val.target.value)}/>
                </FormGroup>
                <Button onClick={() => this.login()}>Log In</Button>
            </Form>
            )
    }
}

export default LoginForm;