import React from 'react';
import axios from 'axios';
import Cookie from 'universal-cookie';

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
                }
            }).then(response => {
                var result = JSON.parse(response.data['responseBody'][0]);
                console.log(result)
                const postList = result.map(post => {
                    return (
                        <div key={post.PostId}>
                            <span><a><b>{post.Username}</b></a></span> <span>{post.CreateDateTime}</span>
                            <br />
                            <p>{post.Body}</p>
                        </div>
                        )
                })

                
                this.setState({ posts: postList} )
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
