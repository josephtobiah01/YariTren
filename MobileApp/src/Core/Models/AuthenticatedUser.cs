using System;
using System.Collections.Generic;
using SQLite;

namespace Core.Models
{
    public class AuthenticatedUser
    {
        [PrimaryKey]
        //public int PersonId { get; set; }
        //public string Username { get; set; }
        //public string EncryptedPassword { get; set; }
        public string EncryptedPinHash { get; set; }
        //public string LastName { get; set; }
        //public string FirstName { get; set; }
        //public string AccessToken { get; set; }
    }
}

