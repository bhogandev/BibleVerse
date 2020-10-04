import React from 'react';
import Layout from './components/Layout';
import Home from './pages/Home';
import { Route } from 'react-router';
import { Switch } from 'react-router-dom';
import './css/App.css';
import Landing from './pages/Landing';


/*
*   <Switch>   
*       <Route to Homepage aka landing page />
*       <Route to Profile Page />
*
*
*   </Switch>
*/



const App = (props) => {
    return (
        <Switch>
            <Route path="/" component={Landing} />
        </Switch>
        )
}

export default App;