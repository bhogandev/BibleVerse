import React from 'react';

class Post extends React.Component {
    constructor(props) {
        super(props);

    }

    render() {
        return (
            <div>
                <span><a href="#"><b>{this.props.Username}</b></a></span> <span>{this.props.CreateDateTime}</span>
                <br />
                <p>{this.props.Body}</p>
            </div>
            )
    }
}
export default Post;