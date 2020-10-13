import React from 'react';
import { Route, Redirect } from 'react-router-dom';
import Auth from '../middleware/Auth';

const ProtectedRoute = ({ component: Component, ...rest }) => {
    return (
        <Route {...rest} render={
            props => {
                {/* Here is where authentication check will go. If (auth) pass component else pass redirect*/ }
                if (!Auth.isAuthenticated()) {
                    return <Redirect exact to="/"/>
                } else {
                    return <Component {...props} />;
                }
            }
        }
        />
        )
}

export default ProtectedRoute;