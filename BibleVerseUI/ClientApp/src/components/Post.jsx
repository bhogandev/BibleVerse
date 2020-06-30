import React from 'react';
import { Button } from 'reactstrap';
import Cookies from 'universal-cookie';
import axios from 'axios';
import CreateCommentForm from './CreateCommentForm.jsx';
import Comment from './Comment.jsx';

class Post extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            photos: null,
            p: null,
            videos: null,
            v: null,
            attachments: JSON.parse(this.props.Attachments),
            IsLiked: this.props.IsLiked,
            postUpdate: false,
            likes: this.props.Likes,
            comments: this.props.Comments,
            CExt: JSON.parse(this.props.CExt),
            coms: null
        }
        
        

    }

    componentDidMount() {
        //console.log(this.state.CExt);
        if (this.state.CExt != null) {
            var comsState = this.state.CExt;
            const coms = comsState.map(com => {
                return <Comment key={com.Id} CommentUserId={com.CommentUserId} Body={com.Body} CreateDateTime={com.CreateDateTime} />
            });

            this.setState({ coms: coms });
        }


        if (this.props.Attachments != null) {
            var p = [];
            var v = [];

            for (var i = 0; i < this.state.attachments.length; i++) {
                if (this.state.attachments[i]['ContentType'] == "Photo") {
                    p.push(this.state.attachments[i]);
                } else {
                    v.push(this.state.attachments[i]);
                }

            }
            this.setState({ photos: p, videos: v });


            if (p != [] || v != []) {
                const photos = p.map(attachment => {
                    return (
                        <img key={attachment['AttachmentID']} src={attachment["Link"]} />
                    )
                });

                this.setState({ p: photos });
   
                const videos = v.map(attachment => {
                    return (
                        <video key={attachment['AttachmentID']} src={attachment["Link"]} control>
                            Your browser does not support the video tag.
                        </video>
                        )
                })
                

            }
        }
    }

    componentDidUpdate() {
        if (this.state.postUpdate) {
            this.UpdatePost();
            this.setState({ postUpdate: false });
        }

        
    }

    async Like(IsLiked) {
        var counter = this.state.likes;
        var invertLike;

        if (IsLiked == "Like") {
            counter++;
            invertLike = "Unlike";
        } else {
            counter--;
            invertLike = "Like";
        }
        this.setState({ likes: counter, IsLiked: invertLike });

        const cookies = new Cookies();

        this.setState({ btnDisabled: true });

        try {

            let res = await (await fetch("https://localhost:5001/api/Post/Interact", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Token': cookies.get('token'),
                    'IntType': IsLiked
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    ParentId: this.props.PostId,
                    LikeType: IsLiked
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
            } else if (res && res.status == "409") {
                var result = await JSON.parse(await res.json());
                if (IsLiked == "Like") {
                    counter--;
                    this.setState({ IsLiked: "Like" });
                } else {
                    counter++;
                    this.setState({ IsLiked: "Unlike" });
                }
                this.setState({ likes: counter });
            } else {
            }
        } catch (exception) {
            if (IsLiked == "Like") {
                counter--;
                this.setState({ IsLiked: "Like" });
            } else {
                counter++;
                this.setState({ IsLiked: "Unlike" });
            }
            this.setState({ likes: counter });
            console.log(exception);
        }


    }

    render() {
        return (
            <div>
                <span><a href="#"><b>{this.props.Username}</b></a></span> <span>{this.props.CreateDateTime}</span>
                <br />
                {this.state.p}
                {this.state.v}
                <p>{this.props.Body}</p>
                <span>{this.state.likes} Likes</span> <span>{this.props.Comments} Comments</span>
                <br />
                {this.state.coms}
                <Button onClick={() => this.Like(this.state.IsLiked)}>{this.state.IsLiked}</Button>
                <CreateCommentForm PostId={this.props.PostId}/>
            </div>
            )
    }
}
export default Post;