import React from 'react';
import Layout from './components/Layout';
import Home from './pages/Home';
import { Route } from 'react-router';
import { Switch } from 'react-router-dom';
import './css/App.css';
import Landing from './pages/Landing';
import ProtectedRoute from './components/ProtectedRoute';
import Profile from './components/Profile';


/*
*   <Switch>   
*       <Route to Homepage aka landing page />
*       <Route to Profile Page />
*   </Switch>
*/



const App = (props) => {
    return (
        <Switch>
            <Route path="/" exact component={Landing} />
            {/* Routes below should be authenticated routes only (involve some type of auth middleware)*/}
            <ProtectedRoute path="/home" component={Home} />
            <ProtectedRoute path="/profile" component={Profile} />
            {/* Routes above should be authenticated routes only (involve some type of auth middleware)*/}
        </Switch>
        )
}

export default App;