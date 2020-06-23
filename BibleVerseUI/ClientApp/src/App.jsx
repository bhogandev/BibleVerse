import React from 'react';
import Layout from './components/Layout';
import Home from './components/Home';
import { Route } from 'react-router';
import BBVHome from './components/BBVHome';
import Cookies from 'universal-cookie';


class App extends React.Component {
    static displayName = App.name;

    constructor(props) {
        super(props);

        const cookie = new Cookies();

        //Here is where you would check the user's cookies and set the user to be signed in (via user state object) if the cookie has not already expired

        this.state = { userIsAuthenticated: cookie.get('token') };
    }

    

    render() {
        /*
         * Here I need to list the layout with the routes for each page
         */
        if (this.state.userIsAuthenticated == null) {
            return (
                <Layout default={true}>
                    <Route exact path='/' component={Home} />
                </Layout>
            )
        }

        if (this.state.userIsAuthenticated != null) {
            return (
                <Layout default={false}>
                    <Route exact path='/' component={BBVHome} />
                </Layout>
            )
        }

        return (
            <div>
                <p>Error</p>
            </div>
            )
    
    }
}

export default App;