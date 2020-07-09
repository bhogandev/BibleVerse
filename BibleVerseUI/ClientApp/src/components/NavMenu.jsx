import React from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, Modal, Button } from 'reactstrap';
import { Link } from 'react-router-dom';
import LogOutBtn from './LogOutBtn';

class NavMenu extends React.Component {
    static displayName = NavMenu.name;

    constructor(props) {
        super(props);

        this.toggleNavbar = this.toggleNavbar.bind(this);
        this.state = {
            collapsed: true,
            layout: this.props.layout,
            showModal: false
        };
    }

    toggleNavbar() {
        this.setState({
            collapsed: !this.state.collapsed
        });
    }


    openModal() {
        !this.state.showModal ? this.setState({ showModal: true }) : this.setState({ showModal: true });
    }

    render() {
    /*Write conditional navs for both layouts*/

        if (this.props.layout) {

            return (
                <header>
                    <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                        <Container>
                            <NavbarBrand tag={Link} to="/">BibleVerse</NavbarBrand>
                            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                                <ul className="navbar-nav flex-grow">
                                    <NavItem>
                                        <NavLink tag={Link} className="text-dark" to="../../public/about.html">About</NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <Button onClick={() => this.openModal()}>Open Modal</Button>
                                        <Modal show={this.state.showModal}>
                                        <Modal.Dialog>
                                            <Modal.Header closeButton>
                                                <Modal.Title>This is a  string</Modal.Title>
                                            </Modal.Header>

                                            <Modal.Body>
                                                <p>Let's try thist thing</p>
                                            </Modal.Body>

                                            <Modal.Footer>
                                                {/*The footer would go herew*/}
                                            </Modal.Footer>
                                              
                                            </Modal.Dialog>
                                            </Modal>
                                    </NavItem>
                                </ul>
                            </Collapse>
                        </Container>
                    </Navbar>
                </header>
            )
        }


        if (!this.props.layout) {
            return (
                <header>
                    <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
                        <Container>
                            <NavbarBrand tag={Link} to="/">BibleVerse</NavbarBrand>
                            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
                            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
                                <ul className="navbar-nav flex-grow">
                                    <NavItem>
                                        <LogOutBtn />
                                    </NavItem>
                                </ul>
                            </Collapse>
                        </Container>
                    </Navbar>
                </header>
            )
        }
    }
}

export default NavMenu;