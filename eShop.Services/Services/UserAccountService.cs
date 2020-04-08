using eShop.Core.Contracts;
using eShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web;
using System.Data.Entity;
using eShop.Core.ViewModels;

namespace eShop.Services
{
    public class UserAccountService
    {
        private IRepository<UserAccount> _contextUserAccount;
        private IRepository<UserRole> _contextUserRole;
        private IRepository<UserAddress> _contextUserAddress;
        private IRepository<Order> _contextOrder;
        private IRepository<OrderItem> _contextOrderItem;

        public UserAccountService(IRepository<UserAccount> userAccount, IRepository<UserRole> userRole, 
            IRepository<UserAddress> userAddress, IRepository<Order> order, IRepository<OrderItem> orderItem)
        {
            _contextUserAccount = userAccount;
            _contextUserRole = userRole;
            _contextUserAddress = userAddress;
            _contextOrder = order;
            _contextOrderItem = orderItem;
        }

        /// <summary>
        /// Method to compute a SHA256 
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
        /// Method to check existance of username and email
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public string ValidateUsernameEmail(string userName, string email)
        {
            string registerMessage = null;

            //selected User based on inputted username
            UserAccount selectedUserAccountforUsername = _contextUserAccount.Collection().SingleOrDefault(ua => ua.UserName == userName);

            //selected User based on inputted email
            UserAccount selectedUserAccountforEmail = _contextUserAccount.Collection().SingleOrDefault(ua => ua.Email == email);

            if (selectedUserAccountforUsername != null && selectedUserAccountforEmail != null)
                registerMessage = "Username & Email are taken, please use other Username & Email.";
            else if (selectedUserAccountforUsername != null)
                registerMessage = "Username is taken, please use other username.";
            else if (selectedUserAccountforEmail != null)
                registerMessage = "Email is taken, please use other Email.";

            return registerMessage;
        }

        /// <summary>
        /// Method to register new user
        /// </summary>
        /// <param name="registerFormModel"></param>
        public string RegisterUser(RegisterFormViewModel registerFormViewModel)
        {
            string registerMessage = ValidateUsernameEmail(registerFormViewModel.UserName, registerFormViewModel.Email);

            if (registerMessage == null)
            {
                //Valid username & email (New User)
                UserAccount inputtedUserAccount = new UserAccount();
                inputtedUserAccount.UserAccountID = Guid.NewGuid();
                inputtedUserAccount.UserName = registerFormViewModel.UserName;
                inputtedUserAccount.UserPassword = ComputeSha256Hash(registerFormViewModel.UserPassword);
                inputtedUserAccount.FirstName = registerFormViewModel.FirstName;
                inputtedUserAccount.LastName = registerFormViewModel.LastName;
                inputtedUserAccount.Email = registerFormViewModel.Email;
                inputtedUserAccount.UserRoleID = _contextUserRole.Collection().SingleOrDefault(ur => ur.UserRoleName == "Customer").UserRoleID;
                inputtedUserAccount.IsActive = true;
                _contextUserAccount.Insert(inputtedUserAccount);
                _contextUserAccount.Commit();
            }

            return registerMessage;
        }

        /// <summary>
        /// Method to check existance of user
        /// </summary>
        /// <param name="LoginFormViewModel"></param>
        /// <returns></returns>
        public UserAccount LoginUser(LoginFormViewModel loginFormViewModel)
        {
            string inputtedUserName = loginFormViewModel.UserName;
            string inputtedPassword = ComputeSha256Hash(loginFormViewModel.UserPassword);
            UserAccount selectedUser = _contextUserAccount.Collection().Include(ua => ua.UserRole)
                .SingleOrDefault(ua => ua.UserName == inputtedUserName && ua.UserPassword == inputtedPassword 
                && ua.UserRole.IsActive == true);
            return selectedUser;
        }

        /// <summary>
        /// Method to check UserRole based on UserAccountID
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <returns></returns>
        public string CheckUserRole(Guid? userAccountID)
        {
            UserAccount userAccount = _contextUserAccount.Collection().Include(ua => ua.UserRole).SingleOrDefault(ua => ua.UserAccountID == userAccountID);
            string userRole = userAccount.UserRole.UserRoleName;
            return userRole;
        }

        /// <summary>
        /// Method to get model data for Edit Profile page based on userAccountID
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <returns></returns>
        public EditProfileFormViewModel GetEditProfileFormModel(Guid? userAccountID)
        {
            UserAccount selectedUserAccount = _contextUserAccount.Collection().Include(ua => ua.UserRole).SingleOrDefault(ua => ua.UserAccountID == userAccountID);
            EditProfileFormViewModel model = new EditProfileFormViewModel();
            model.UserName = selectedUserAccount.UserName;
            model.UserAccountID = selectedUserAccount.UserAccountID;
            model.FirstName = selectedUserAccount.FirstName;
            model.LastName = selectedUserAccount.LastName;
            return model;
        }

        /// <summary>
        /// Method to get view model data for User Account page based on userAccountID
        /// </summary>
        /// <param name="userAccountID"></param>
        /// <returns></returns>
        public UserAccountViewModel GetUserAccountViewModel(Guid? userAccountID)
        {
            UserAccountViewModel viewModel = new UserAccountViewModel();
            viewModel.userAccount = _contextUserAccount.Collection().Include(ua => ua.UserRole).SingleOrDefault(ua => ua.UserAccountID == userAccountID);
            viewModel.userAddresses = _contextUserAddress.Collection().Where(uad => uad.UserAccountID == userAccountID && uad.IsActive == true).ToList();
            viewModel.orders = _contextOrder.Collection().Where(o => o.UserAccountID == userAccountID).ToList();

            return viewModel;
        }

        /// <summary>
        /// Method to add new address
        /// </summary>
        /// <param name="addUserAddressFormModel"></param>
        /// <param name="userAccountID"></param>
        public void AddUserAddress(UserAddressFormModel addUserAddressFormModel, Guid? userAccountID)
        {
            UserAddress inputtedUserAddress = new UserAddress();
            inputtedUserAddress.UserAddressID = Guid.NewGuid();
            inputtedUserAddress.UserAccountID = userAccountID;
            inputtedUserAddress.CreatedAt = DateTime.Now;
            inputtedUserAddress.IsActive = true;
            inputtedUserAddress.Address = addUserAddressFormModel.Address;
            inputtedUserAddress.Country = addUserAddressFormModel.Country;
            inputtedUserAddress.Region = addUserAddressFormModel.Region;
            inputtedUserAddress.City = addUserAddressFormModel.City;
            _contextUserAddress.Insert(inputtedUserAddress);
            _contextUserAddress.Commit();
        }

        /// <summary>
        /// Method to get data for Edit address form
        /// </summary>
        /// <param name="addUserAddressFormModel"></param>
        /// <param name="userAccountID"></param>
        public UserAddressFormModel GetEditUserAddressForm(Guid UserAddressID)
        {
            UserAddress selectedUserAddress = _contextUserAddress.Find(UserAddressID);
            UserAddressFormModel viewModel = new UserAddressFormModel()
            {
                UserAddressID = selectedUserAddress.UserAddressID,
                Address = selectedUserAddress.Address,
                Country = selectedUserAddress.Country,
                Region = selectedUserAddress.Region,
                City = selectedUserAddress.City
            };
            return viewModel;
        }

        /// <summary>
        /// Method to Edit address
        /// </summary>
        /// <param name="addUserAddressFormModel"></param>
        /// <param name="userAccountID"></param>
        public void EditUserAddress(UserAddressFormModel UserAddressFormModel)
        {
            UserAddress selectedUserAddress = _contextUserAddress.Find(UserAddressFormModel.UserAddressID);
            selectedUserAddress.Address = UserAddressFormModel.Address;
            selectedUserAddress.Country = UserAddressFormModel.Country;
            selectedUserAddress.Region = UserAddressFormModel.Region;
            selectedUserAddress.City = UserAddressFormModel.City;
            selectedUserAddress.ModifiedAt = DateTime.Now;
            _contextUserAddress.Commit();
        }

        /// <summary>
        /// Method to Delete address
        /// </summary>
        /// <param name="addUserAddressFormModel"></param>
        /// <param name="userAccountID"></param>
        public void DeleteUserAddress(Guid UserAddressID)
        {
            UserAddress selectedUserAddress = _contextUserAddress.Find(UserAddressID);
            selectedUserAddress.ModifiedAt = DateTime.Now;
            selectedUserAddress.IsActive = false;
            _contextUserAddress.Commit();
        }
    }
}
