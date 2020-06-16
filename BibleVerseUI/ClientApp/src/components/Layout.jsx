import React from 'react'
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';

/*
 * Here I need to create the layout for the page
 */

class Layout extends React.Component {
    static displayName = Layout.name;

    constructor(props) {
        super(props);

        this.state = { default: this.props.default };
    }

    render() {

        if (!this.state.default) {
            return (
                <div>
                    {/*Here is where the default layout nav will go (i.e. home page nav and static page navs)*/}
                    <NavMenu layout={false}/>
                    <Container>
                        {this.props.children}
                    </Container>
                </div>
                )
        }

        return (
            <div>
                {/*Here is where the default layout nav will go (i.e. home page nav and static page navs)*/}
                <NavMenu layout={true}/>
                <Container>
                    {this.props.children}
                </Container>
            </div>
            )
    }

}

export default Layout;