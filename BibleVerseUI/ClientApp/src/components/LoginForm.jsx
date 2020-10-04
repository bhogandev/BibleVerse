import React from 'react';
import Spinner  from '../components/Spinner';
import { Button, Form, FormGroup, Label, Input, FormText } from 'reactstrap';
import Cookies from 'universal-cookie';

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

    componentDidUpdate() {
        if (document.getElementById("errors") != null) {
            this.state.errors.forEach(
                x => document.getElementById("errors").innerHTML = (x["Description"])
            )
        }
    }


    async login() {
        
        //window.console.log("clicked");

        const cookies = new Cookies();

        this.setState({ btnDisabled: true, loading: true });

        try {

            let res = await (await fetch("https://localhost:5001/api/Login/LoginUser", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    Email: this.state.email,
                    Password: this.state.password
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
                var result = await JSON.parse(await res.json());

                //Set JWT cookie in browser cookie storage
                cookies.set('token', (result["Token"]));
                //Set Refresh cookie in browser cookie storage
                cookies.set('refreshToken', result['RefreshToken']);
                cookies.set('user', result['User']);

                //refresh page
                window.location.reload(false);

            } else if (res && res.status == "409") {
                var result = await JSON.parse(await res.json());

                this.setState({ errors: result["ResponseErrors"], loading: false });
            } else {
                var defaultError = [{
                    "Description": "An Unexpected Error Has Occured, Please try again!"
                }]

                this.setState({ errors: defaultError, loading: false });

            }
        } catch (exception) {
            console.log(exception);
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
                        <Button style={{textAlign:"center"}} onClick={() => this.login()}>Log In</Button>
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