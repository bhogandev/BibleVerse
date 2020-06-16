import React from 'react'
import LoginForm from './LoginForm';

class Home extends React.Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                <p>This is the home screen</p>
                <a href="about.html">Go to the about page</a>

                {/*Here is where the Login Form will go*/}
                <LoginForm />
            </div>
            )
    }
}

export default Home;