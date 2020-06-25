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
        };
    }

    componentDidMount() {

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
                    console.log(result)
                    const postList = result.map(post => {
                        
                        return (
                            <Post key={post.PostId} Username={post.Username} CreateDateTime={post.CreateDateTime} Body={post.Body} />
                        )
                    })
                    this.setState({ posts: postList })
                }
            }).catch(error => {
                console.log(error);
                cookie.remove('token');
                window.location.reload();
            });

            console.log(res);
        } catch (Ex) {
            console.log(Ex);
        }
    }


    render() {
        if (this.state.posts != null) {
            return (
                <div>
                    <CreatePostForm />
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
