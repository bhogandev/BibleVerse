using System;

namespace BVCommon
{
    public class BVFunctions
    {
        // Generate New User ID
        public static string CreateUserID()
        {
            string generatedUserId = "";
            int uidLength = 21;


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

                if (generatedUserId.Length < 32)
                {
                    generatedUserId += '-'; // Add a - for serialization
                }
            }

            return generatedUserId; // Return Generated User Id
        }
    }
}
