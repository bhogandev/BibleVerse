import React from 'react';
import axios from 'axios';
import bbvapi from '../middleware/BBVAPI';
import Cookie from 'universal-cookie';
import Post from './Post';
import CreatePostForm from './CreatePostForm';
import { Container } from 'react-bootstrap'
import BBVAPI from '../middleware/BBVAPI';
import { type } from 'jquery';

class Timeline extends React.Component {
  static displayName = Timeline.name;

    constructor(props) {
        super(props);

        this.state = {
            posts: null,
            updateTl: false,
            user: this.props.user
        };

        this.tlupdate = this.tlupdate.bind(this);
    }

    tlupdate() {
        this.setState({ updateTl: true });
    }

    async GetTL() {
        //Abstract this function to api call in bbvapi
        let cookie = new Cookie();
        try {
           var response = await bbvapi.getUserTimeline(cookie.get('token'), cookie.get('refreshToken'));

            if (typeof (await response) == typeof ('')) {
                //return error to timeline
            } else {
                if (await response['responseMessage'] != 'Success'.toUpperCase()) {
                    console.log(await response['responseMessage']);
                    cookie.remove('token');
                    window.location.reload();
                } else {
                    const postList = await JSON.parse(response["responseBody"]).map(post => {
                        var parsedCExt = post.CommentsExt;
                        //console.log(parsedCExt);
                        var user = cookie.get('user');

                        var isOwner = false;
                        if (post.Username == user['UserName']) {
                            isOwner = true;
                        }

                       
                        return (
                            <Post key={post.PostId} PostId={post.PostId} Username={post.Username} CreateDateTime={post.CreateDateTime} Body={post.Body} Attachments={post.Attachments} Likes={post.Likes} IsLiked={post.LikeStatus} Comments={post.Comments} CExt={JSON.parse(post.CommentsExt)} isOwner={isOwner} GetTL={() => this.GetTL()} />
                        )
                    })
                    this.setState({ posts: postList })
                    //console.log(postList);
                }
            }
        } catch (Ex) {
            console.log(Ex);
        }
    }

    componentDidMount() {
        this.GetTL();
    }

    componentDidUpdate() {
        if (this.state.updateTl) {
            this.GetTL();
            this.setState({ updateTl: false });
        }
    }


    render() {
        if (this.state.posts != null) {
            return (
                <Container style={{ textAlign: "left" }}>
                    <CreatePostForm tlUpdate={this.tlupdate}/>
                    <h1>{this.state.posts}</h1>
                </Container>
            );
        }
        return (
            <div><p>Loading...</p></div>
            )
  }
}

export default Timeline;
