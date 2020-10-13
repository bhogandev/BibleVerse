import BBVAPI from './BBVAPI';
import bbvapi from './BBVAPI';
import Cookies from 'universal-cookie';

class Auth {
    constructor() {
        //Write logic to set authenticated based on refreshToken
        this.authenticated = false;
        this.errors = [];
    }

    async refreshAuth() {
        var cookie = new Cookies();

        if(cookie.get('token') != null && cookie.get('refreshToken') != null)
        {
            //Make request here to attempt login with token + refreshToken
            //If successfull, then authenticated = true + new user cookie will be returned and set in BBVAPI
           var response = await bbvapi.validateRefreshToken(cookie.get('token'), cookie.get('refreshToken'))
                
            if(typeof(response) == "array")
                {
                //return to homepage and pass along err message
                //Find a way to return err messages
                return this.authenticated;
                } else {
                    //Allow user to navigate to route and continue with action
                this.authenticated = true;
                return this.authenticated;
                }
        }

    }

    async login(e, p) {
        this.errors = await bbvapi.login(e, p)
        console.log(this.errors);
        if(this.errors.length == 0)
        {
            this.authenticated = true;
            return this.authenticated;
        } else {
            return this.errors;
        }
    }

    async logout(t) {
        this.errors = bbvapi.logout(t)
        console.log(this.errors);
        if(this.errors != [])
        {
            this.authenticated = false;
        } else {
            return this.errors;
        }
    }

    isAuthenticated() {
        return this.authenticated;
    }

}

export default new Auth();