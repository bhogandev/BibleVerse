using System;


namespace BVCommon
{
    public class BVFunctions
    {
        // Generate New User ID
        public static string CreateUserID()
        {
            string generatedUserId = "";
            int uidLength = 27;


            string[] alpha = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
            int[] numeric = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

            while(generatedUserId.Length < uidLength) // While GUID != 30 (Not Complete) Loop!
            {
                Random random = new Random();
                int ranNum = random.Next(0, 10);

                for (int i = 0; i < 6; i++)
                {
                    if (ranNum % 2 == 0)
                    {
                        generatedUserId += alpha[random.Next(0, alpha.Length)].ToUpper();
                    }
                    else
                    {
                        generatedUserId += numeric[random.Next(0, numeric.Length)].ToString();
                    }
                }

                if (generatedUserId.Length < 27)
                {
                    generatedUserId += '-'; // Add a - for serialization
                }
            }

            return generatedUserId; // Return Generated User Id
        }

        // Find User Age
        public static int GetUserAge(DateTime userDOB)
        {
            DateTime dateToday = DateTime.Today;

            int userAge = dateToday.Year - userDOB.Year;

            if(dateToday.DayOfYear < userDOB.DayOfYear) { userAge--;  }

            return userAge;

        }

        //Create Referal Code For Org
        public static string CreateRefCode()
        {
            string refCode = "";
            int refCodeLength = 12;

            string[] alpha = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            int[] numeric = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            while (refCode.Length < refCodeLength) // While GUID != 30 (Not Complete) Loop!
            {
                Random random = new Random();
                int ranNum = random.Next(0, 10);

                for (int i = 0; i < 6; i++)
                {
                    if (ranNum % 2 == 0)
                    {
                        refCode += alpha[random.Next(0, alpha.Length)].ToUpper();
                    }
                    else
                    {
                        refCode += numeric[random.Next(0, numeric.Length)].ToString();
                    }
                }
            }

            return refCode;
        }

        //Get User Status From Referral Code
        public static string RetreiveStatusFromRefCode(string refCodeType)
        {
            if(refCodeType == "Owner Referral Code")
            {
                return "Owner";
            } else
            {
                return "Error: Not Valid Code";
            }
        }

        //Return Relationship Request Type
        public static string RetrieveRelationshipType(string relType)
        {
            var returnType = "";

            switch (relType)
            {
                case "Send friend request": returnType = "FRIEND"; break;
                case "Cancel friend request": returnType = "FRIEND"; break;
                case "Accept friend request": returnType = "FRIEND"; break;
                default: returnType = "Error: Invalid Relationship Statement"; break;
            }

            return returnType;
        }

        //Create init file for dirs in buckets
        public static string CreateInit(string masterName)
        {
            string initBody;
            initBody = "Dir Master:" + masterName + "\n" + "Created On:" + DateTime.Now.ToString();
            return initBody;
        }
    }
}
