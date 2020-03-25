using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BVCommon
{
    public class ELogFunctions
    {
        public static List<string> GetSignInError(SignInResult signInResult)
        {
            List<string> error = new List<string>();

            if(signInResult.IsLockedOut)
            {
                error.Add("IsLockedOut");
                error.Add("User has been locked out");
            } else if(signInResult.IsNotAllowed)
            {
                error.Add("IsNotAllowed");
                error.Add("Sign in is not allowed at this time");
            } else if(signInResult.RequiresTwoFactor)
            {
                error.Add("RequiresTwoFactor");
                error.Add("Two factor authentication required");
            } else
            {
                error.Add("N/A");
                error.Add("N/A");
            }

            if(error.Count > 2) //Make sure there's only one error being sent back
            {
                while(error.Count > 2)
                {
                    error.RemoveAt(2);
                }
            }

            return error;
        }
    }
}
