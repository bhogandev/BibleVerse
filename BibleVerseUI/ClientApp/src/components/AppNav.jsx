import React from 'react';
import {Nav, Navbar, NavDropdown ,Button, Container, Form} from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';
import SearchBar from './SearchBar';

/**
 *  - Grouper Logo (Home Nav link)
 *  - Menu Sandwich (Collapsed)
 *  - SearchBar (users, groups, videos, etc)
 *  - User Acct dropdown
 *  - Inbox
 *  - Notifications
 */

class AppNav extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Navbar bg={this.props.navBG} expand="md" variant={this.props.navBG} style={{"width": "100vw"}}>
                <LinkContainer to="/home">
                    <Navbar.Brand>
                        Grouper Logo Here    
                    </Navbar.Brand>
                </LinkContainer>
                <Nav className="mr-auto">
                    <NavDropdown title="supertitle" id="basic-nav-dropdown">
                        <NavDropdown.Item>tea</NavDropdown.Item>
                    </NavDropdown>
                    <Nav.Item>
                        <SearchBar />
                    </Nav.Item>
                </Nav>
            </Navbar>
        )
    }
}

export default AppNav;