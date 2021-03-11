import React from 'react';
import Cookies from 'universal-cookie';
import BBVAPI from '../middleware/BBVAPI';
import CreatePostForm from './CreatePostForm';
import {Container} from 'react-bootstrap';

class Profile extends React.Component {
    static displayName = Profile.name;

    constructor(props){
        super(props);

        this.state = {
            posts: null,
            updateProfile: false,
            user: window.location.pathname.substring(9),
            profile: null
        };
    }

    storeUser(){
        try{
            var userName = window.location.pathname.substring(9);
        }catch(ex){
            let cookies = new Cookies();

            userName = cookies.get('user')['UserName'];
        }
      console.log(userName);
    }

    profileUpdate()
    {
        this.setState({updateProfile: true});
    }

    async GetProfile(){
        let cookie = new Cookies();

        try{
            var response = await BBVAPI.getProfile(cookie.get('token') , cookie.get('refreshToken'), this.state.user);

            if(typeof (await response) == typeof ('')){
                //send user to error page
            } else {
                if (await response['responseMessage'] != 'Success'.toUpperCase()){
                    console.log(await response['responseMessage']);
                    cookie.remove('token');
                    //window.location.reload();
                } else {
                    await this.setState({profile: response["responseBody"][0]});

                }
            }
        }catch(ex){

        } 
    }

    componentDidMount(){
        this.GetProfile();
    }

    componentDidUpdate(){
        
        if(this.state.updateProfile){
            this.GetProfile();
            this.setState({updateProfile: false});
        }
        
    }

    render(){
        if (this.state.profile != null) {
            return (
                <Container style={{ textAlign: "left" }}>
                    <CreatePostForm tlUpdate={this.profileUpdate}/>
                    <h1>{this.state.profile["UserName"]}</h1>
                </Container>
            );
        }
        return (
            <div><p>Loading...</p></div>
            )
    }
}

export default Profile;