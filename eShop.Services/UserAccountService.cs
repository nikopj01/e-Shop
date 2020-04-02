using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web;

namespace eShop.Services
{
    public class UserAccountService
    {
        private IRepository<UserAccount> _contextUserAccount;

        public UserAccountService(IRepository<UserAccount> userAccount)
        {
            _contextUserAccount = userAccount;
        }

        /// <summary>
        /// Method to check existance of username and email
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public string ValidateUsernameEmail(string userName, string email)
        {
            string registerMessage = null;

            //Check username
            UserAccount selectedUserAccountforUsername = _contextUserAccount.Collection().SingleOrDefault(ua => ua.UserName == userName);
            //Check email
            UserAccount selectedUserAccountforEmail = _contextUserAccount.Collection().SingleOrDefault(ua => ua.Email == email);

            if (selectedUserAccountforUsername != null && selectedUserAccountforEmail != null)
                registerMessage = "Username & Email are taken, please use other Username & Email.";
            else if (selectedUserAccountforUsername != null )
                registerMessage = "Username is taken, please use other username.";
            else if (selectedUserAccountforEmail != null)
                registerMessage = "Email is taken, please use other Email.";

            return registerMessage;
        }

        /// <summary>
        /// Method to create a SHA256 
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        public string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Method to register new user
        /// </summary>
        /// <param name="registerFormModel"></param>
        public void RegisterUser(RegisterFormModel registerFormModel)
        {
            UserAccount inputtedUserAccount = new UserAccount();
            inputtedUserAccount.UserAccountID = Guid.NewGuid();
            inputtedUserAccount.UserName = registerFormModel.UserName;
            inputtedUserAccount.UserPassword = ComputeSha256Hash(registerFormModel.UserPassword); 
            inputtedUserAccount.FirstName = registerFormModel.FirstName;
            inputtedUserAccount.LastName = registerFormModel.LastName;
            inputtedUserAccount.Email = registerFormModel.Email;
            _contextUserAccount.Insert(inputtedUserAccount);
            _contextUserAccount.Commit();
        }

        public Guid? LoginUser(LoginFormModel loginFormModel)
        {
            string inputtedUserName = loginFormModel.UserName;
            string inputtedPassword = ComputeSha256Hash(loginFormModel.UserPassword);
            UserAccount user = _contextUserAccount.Collection().SingleOrDefault(ua => ua.UserName == inputtedUserName && ua.UserPassword == inputtedPassword);
            if (user != null)
            {
                return user.UserAccountID;
            }
            return null;
        }
    }
}
