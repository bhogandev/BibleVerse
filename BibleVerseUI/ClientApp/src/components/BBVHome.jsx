import React from 'react';
import axios from 'axios';
import Cookie from 'universal-cookie';
import Post from './Post';
import CreatePostForm from './CreatePostForm';

class BBVHome extends React.Component {
  static displayName = BBVHome.name;

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
        let cookie = new Cookie();

        try {
            let res = axios.get('https://localhost:5001/api/Post/GetTimeline', {
                headers: {
                    Token: cookie.get('token')
                },
                validateStatus: () => true
            }).then(response => {
                if (response.status != '200') {
                    console.log(response.status);
                    cookie.remove('token');
                    window.location.reload();

                } else {
                    var result = JSON.parse(response.data['responseBody'][0]);
                    //console.log(result);
                    const postList = result.map(post => {
                        var parsedCExt = JSON.parse(post.CommentsExt);
                        //console.log(parsedCExt);
                        var user = cookie.get('user');
                        
                        var isOwner = false;
                        if (post.Username == user['UserName']) {
                            isOwner = true;
                        }
                        return (
                            <Post key={post.PostId} PostId={post.PostId} Username={post.Username} CreateDateTime={post.CreateDateTime} Body={post.Body} Attachments={post.Attachments} Likes={post.Likes} IsLiked={post.LikeStatus} Comments={post.Comments} CExt={post.CommentsExt} isOwner={isOwner} GetTL={() => this.GetTL()}/>
                        )
                    })
                    this.setState({ posts: postList })
                    //console.log(postList);
                }
            }).catch(error => {
                console.log(error);
                //cookie.remove('token');
                //window.location.reload();
            });

            console.log(res);
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
                <div>
                    <CreatePostForm tlUpdate={this.tlupdate}/>
                    <h1>{this.state.posts}</h1>
                </div>
            );
        }
        return (
            <div><p>Loading...</p></div>
            )
  }
}

export default BBVHome;
