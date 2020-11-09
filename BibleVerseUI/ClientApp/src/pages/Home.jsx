import React from 'react'
import {Container, Row, Col} from 'react-bootstrap';
import { Button } from 'react-bootstrap';
import Cookies from 'universal-cookie';
import Sidebar from '../components/Sidebar';
import Timeline from '../components/Timeline';
import AppNav from '../components/AppNav';



/*
                *   -AppNav
                *   -(Col) Sidebar
                *   - (Col) TL(Feed)
                *   -(Col) Extra + Ads
                */

class Home extends React.Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        var cookies = new Cookies();
        this.state = {
            user: cookies.get('user')
        }

    }

    componentDidMount(){
        
    }

    render() {
        return (
            <div>
            <AppNav navBG="light"/>
            <Container fluid>
                <Row>
                    <Col>
                        <Sidebar />
                    </Col>
                    <Col>
                        <Timeline />
                    </Col>
                    <Col>
                         <Sidebar />
                    </Col>
                 </Row>
                </Container>
                </div>
            )
    }
}

export default Home;