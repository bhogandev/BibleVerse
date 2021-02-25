import React from 'react';
import { Button, Form, FormGroup, Label, Input, FormText } from 'reactstrap';
import Cookies from 'universal-cookie';

class CreatePostForm extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            post: '',
            errors: [],
        };

    }

    setPropVal(prop, val) {
        //val = val.trim();

        this.setState({
            [prop]: val
        });
    }

    async post() {
        const cookies = new Cookies();

        this.setState({ btnDisabled: true });

        try {

            let res = await (await fetch("https://localhost:44307/api/Post/CreatePost", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Token': cookies.get('token')
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    Body: this.state.post
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
                this.setState({ post: '' });
                this.props.tlUpdate();
            } else if (res && res.status == "409") {
                var result = await JSON.parse(await res.json());

                this.setState({ errors: result["ResponseErrors"] });
            } else {
                var defaultError = [{
                    "Description": "An Unexpected Error Has Occured, Please try again!"
                }]

                this.setState({ errors: defaultError });

            }
        } catch (exception) {
            console.log(exception);
        }


    }

    render() {
        return (
            <div>
                <Form>
                    <FormGroup>
                        <Label for="post">Post:</Label>
                        <Input type="textarea" name="post" id="post" placeholder="Type post here" value={this.state.post ? this.state.post : ''} onChange={(val) => this.setPropVal("post", val.target.value)} />
                    </FormGroup>
                    <Button onClick={() => this.post()}>Post</Button>
                </Form>
            </div>
            )
    }
}
export default CreatePostForm;