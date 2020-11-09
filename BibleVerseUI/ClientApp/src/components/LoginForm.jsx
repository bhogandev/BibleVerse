import React from 'react';
import Spinner  from '../components/Spinner';
import { Button, Form, FormGroup, Label, Input, FormText } from 'reactstrap';
import Cookies from 'universal-cookie';
import { Redirect } from "react-router-dom";
import Auth from '../middleware/Auth';

class LoginForm extends React.Component {
    static displayName = LoginForm.name;

    constructor(props) {
        super(props);

        this.state = {
            email: '',
            password: '',
            btnDisabled: false,
            errors: [],
            loading: false
        };
    }

    setPropVal(prop, val) {
        val = val.trim();

        this.setState({
            [prop]: val
        });
    }

    componentWillMount() {
        this.checkLogin();
    }

    componentDidMount() {
        //this.checkLogin();
    }

    componentDidUpdate() {
         if (document.getElementById("errors") != null) {
            this.state.errors.forEach(
                x => document.getElementById("errors").innerHTML = (x["Description"])
            )
        }
    }

    async checkLogin() {
        var result = await Auth.refreshAuth();

        if(result)
        {
            //Send user to dashboard
            await this.props.history.push('./home');
        } 
    }

     async login() {

         this.setState({ btnDisabled: true, loading: true })

         var result = await Auth.login(this.state.email, this.state.password);

         if (result && typeof(result) == "boolean") {
             //Redirect to home
             await this.props.history.push('./home');
         } else {
             await this.setState({ errors: result, loading: false });
         }
    }

     renderForm()
    {
        if(this.state.loading)
        {
            return (
                <Spinner />
            )
        } else {
            return (
                <div>
                <Form>
                    <FormGroup>
                        <div id="errors" style={{ color: "red" }}>
                        </div>
                        <Label for="email">Email</Label>
                    <Input type="email" name="email" id="email" placeholder="Email" value={this.state.email ? this.state.email : ''} onChange={(val) => this.setPropVal("email", val.target.value)}/>
                </FormGroup>
                <FormGroup>
                        <Label for="password">Password</Label>
                    <Input type="password" name="password" id="password" placeholder="Password" value={this.state.password ? this.state.password : ''} onChange={(val) => this.setPropVal("password", val.target.value)}/>
                </FormGroup>
                        <Button style={{ textAlign: "center" }} onClick={() => this.login()}>Log In</Button>
                    </Form>
                    </div>
            )
        }
    }

    render() {
        return (
            <div>
                    {this.renderForm()}
                
            </div>
         )
    }
}

export default LoginForm;