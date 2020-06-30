import React from 'react';

class Comment extends React.Component {
    constructor(props) {
        super(props);
    }

    componentDidMount() {}

    componentDidUpdate() {}

    render() {
        return (
            <div>
                <p><b>{this.props.CommentUserId}</b></p>
                <p><b>{this.props.CreateDateTime}</b></p>
                <p>{this.props.Body}</p>
            </div>
            )
    }
}

export default Comment;