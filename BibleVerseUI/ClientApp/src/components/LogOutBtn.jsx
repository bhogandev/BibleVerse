import React from 'react';
import { Button } from 'reactstrap';
import Cookies from 'universal-cookie';

class LogOutBtn extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            errors: []
        }
    }

    async LogOut() {
        const cookies = new Cookies();

        this.setState({ btnDisabled: true });

        try {

            let res = await (await fetch("https://localhost:5001/api/Logout", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    Token: cookies.get('token')
                },
                credentials: 'same-origin'
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
                cookies.remove('token');
                window.location.reload();
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
            <Button onClick={() => this.LogOut()}>Log Out</Button>
            )
    }
}
export default LogOutBtn;