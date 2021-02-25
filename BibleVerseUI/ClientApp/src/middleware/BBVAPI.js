import axios from 'axios';
import { data } from 'jquery';
import Cookies from 'universal-cookie';

class bbvapi {
    constructor() {
        this.apiBase =  "https://localhost:44307/api/";
    }

    async login(e, p) {
        const cookies = new Cookies();

        try {

            let res = await (await fetch(this.apiBase + "Login/LoginUser", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                    Email: e,
                    Password: p
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
                var result = await JSON.parse(await res.json());

                //Set JWT cookie in browser cookie storage
                cookies.set('token', (result["Token"]));
                //Set Refresh cookie in browser cookie storage
                cookies.set('refreshToken', result['RefreshToken']);
                cookies.set('user', result['User']);

                //Redirect to app dashboard
                return [];

            } else if (res && res.status == "409") {
                var result = await JSON.parse(await res.json());

                //find a way to return errrors and loading state
                //this.setState({ errors: result["ResponseErrors"], loading: false });

                return result["ResponseErrors"];
            } else {
                //find a way to return errrors and loading state
                var defaultError = [{
                    "Description": "An Unexpected Error Has Occured, Please try again!"
                }]

                //this.setState({ errors: defaultError, loading: false });
                return defaultError;
            }
        } catch (exception) {
            //find a way to return errrors and loading state
            console.log(exception);
            var defaultError = [{
                "Description": exception
            }]
            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }

    }

    async validateRefreshToken(t, rt) {
        const cookies = new Cookies();

        try {

            let res = await (await fetch(this.apiBase + "Login/RefreshToken", {
                method: 'post',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                credentials: 'same-origin',
                body: JSON.stringify({
                   AccessToken: t,
                   RefreshToken: rt
                })
            }))

            //Verify fetch completed successfully

            if (res && res.ok) {
                var result = await JSON.parse(await res.json());

                //console.log(result);

                //Set JWT cookie in browser cookie storage
                cookies.set('token', (result["AccessToken"]));
                //Set Refresh cookie in browser cookie storage
                cookies.set('refreshToken', result['RefreshTokens'][0]['Token']);
                cookies.set('user', result);

                //Redirect to app dashboard
                return true;

            } else if (res && res.status == "409") {
                var result = await JSON.parse(await res.json());

                //find a way to return errrors and loading state
                //this.setState({ errors: result["ResponseErrors"], loading: false });

                return result["ResponseErrors"];
            } else {
                //find a way to return errrors and loading state
                var defaultError = [{
                    "Description": "An Unexpected Error Has Occured, Please try again!"
                }]

                //this.setState({ errors: defaultError, loading: false });
                return defaultError;
            }
        } catch (exception) {
            //find a way to return errrors and loading state
            console.log(exception);
            var defaultError = [{
                "Description": exception
            }]
            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }
    }

    async logout(t) {
        alert(t);
    }

    async getUserTimeline(t, rt) {

        const cookies = new Cookies();
        var resOk = false;

        try {
            let res = await (await fetch(this.apiBase + "Post/GetTimeline", {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Token': t,
                    'RefreshToken': rt
                },
                validateStatus: () => true,
                credentials: 'same-origin'
            }));

            //await console.log(await res.json());

            if (await res != null && await res.ok) {
                
                var result = await res.json();
                return result;

            } else if (res && res.status == "409") {
            var result = await res;

            //find a way to return errrors and loading state
            //this.setState({ errors: result["ResponseErrors"], loading: false });

            return await res;
            } else {
            //find a way to return errrors and loading state
            var defaultError = [{
                "Description": "An Unexpected Error Has Occured, Please try again!"
            }]

            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }

        } catch (ex) {

        }
    }

    async getProfile(t, rt, username){
        
        const cookies = new Cookies();
        var resOk = false;

        try {
            let res = await (await fetch(this.apiBase + "User/GetProfile", {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Token': t,
                    'RefreshToken': rt,
                    'UserName': username
                },
                validateStatus: () => true,
                credentials: 'same-origin'
            }));

            //await console.log(await res.json());

            if (await res != null && await res.ok) {
                
                var result = await res.json();
                return result;

            } else if (res && res.status == "409") {
            var result = await res;

            //find a way to return errrors and loading state
            //this.setState({ errors: result["ResponseErrors"], loading: false });

            return await res;
            } else {
            //find a way to return errrors and loading state
            var defaultError = [{
                "Description": "An Unexpected Error Has Occured, Please try again!"
            }]

            //this.setState({ errors: defaultError, loading: false });
            return defaultError;
        }

        } catch (ex) {

        }
    }

    async query(range, val) {
        const cookies = new Cookies();

        try {
            let res = await (await fetch(this.apiBase + "User/Query", {
                method: 'get',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Token': cookies.get('token'),
                    'qFilter': range,
                    'Query': val
                },
                credentials: 'same-origin'
            }));

            if (res && res.ok) {

                var result = await res.json();
                console.log(result)
                return JSON.parse(result["result"]["responseBody"]);

            } else if (res && res.status == "409") {
                var result = await JSON.parse(await res.json());

                //find a way to return errrors and loading state
                //this.setState({ errors: result["ResponseErrors"], loading: false });

                return result["ResponseErrors"];
            } else {
                //find a way to return errrors and loading state
                var defaultError = [{
                    "Description": "An Unexpected Error Has Occured, Please try again!"
                }]

                //this.setState({ errors: defaultError, loading: false });
                return defaultError;
            }

        } catch (ex) {

        }
    }
}

export default new bbvapi();