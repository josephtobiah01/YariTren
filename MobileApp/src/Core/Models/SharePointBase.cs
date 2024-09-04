using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Models
{
    #region List Item classes - nometadata. Use these classes if the 'nometadata' option is set when interacting with SP Rest API - Accept: application/json;odata=nometadata

    public class SPJsonSingleVal
    {
        public string value { get; set; }
    }

    public class SpJsonData<T>
    {
        public List<T> value { get; set; }
    }

    public class URL
    {
        public string Description { get; set; }
        public string Url { get; set; }
    }

    public class SpItem
    {
        public int FileSystemObjectType { get; set; }
        public int Id { get; set; }
        public object ServerRedirectedEmbedUri { get; set; }
        public string ServerRedirectedEmbedUrl { get; set; }
        public string ContentTypeId { get; set; }
        public string Title { get; set; }
        public object ComplianceAssetId { get; set; }
        public string DescriptionLead { get; set; }
        public URL URL { get; set; }
        public int ID { get; set; }
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public int AuthorId { get; set; }
        public int EditorId { get; set; }
        public string OData__UIVersionString { get; set; }
        public bool Attachments { get; set; }
        public string GUID { get; set; }
    }

    public class SPItemCount
    {
        public int ItemCount { get; set; }
    }

    public class Metadata
    {
        public string type { get; set; }
        //public string etag { get; set; }
        //public string id { get; set; }
        //public string uri { get; set; }
    }

    public class SPChoiceCollection<T>
    {
        public SPChoiceCollection(List<T> collection)
        {
            if (collection == null) return;
            var type = collection.GetType();
            if (type == typeof(int))
            {
                SPChoiceType = "Collection(Edm.Int32)";
            }
            if (collection != null)
            {
                results = collection;
            }
        }

        public List<T> results { get; set; } = new List<T>();

        public string SPChoiceType { get; set; } = "Collection(Edm.String)"; // default to string

        public Metadata __metadata
        {
            get
            {
                return new Metadata { type = SPChoiceType };
            }
        }
    }

    #endregion

    #region List Item Classes - verbose. Use these classes if the verbose option is set when interacting with SP Rest API - Accept: application/json;odata=verbose

    public class SpJsonDataVerbose<T>
    {
        public T d { get; set; }
    }

    public class SpJsonDataResults<T>
    {
        public T results { get; set; }
    }

    #endregion

    #region UserProfile Data

    public class UserProfileProperty
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
    }

    public class UserProfile
    {
        public string AccountName { get; set; }
        public List<object> DirectReports { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public List<string> ExtendedManagers { get; set; }
        public List<string> ExtendedReports { get; set; }
        public bool IsFollowed { get; set; }
        public object LatestPost { get; set; }
        public List<string> Peers { get; set; }
        public string PersonalSiteHostUrl { get; set; }
        public string PersonalUrl { get; set; }
        public string PictureUrl { get; set; }
        public object Title { get; set; }
        public List<UserProfileProperty> UserProfileProperties { get; set; }
        public string UserUrl { get; set; }

        public string MobileNumber
        {
            get
            {
                return GetProperty("CellPhone");
            }
        }

        public string EmployeeNumber
        {
            get
            {
                return GetProperty("EmployeeNumber");
            }
        }

        public string EmployeeRole
        {
            get
            {
                return GetProperty("EmployeeRole");
            }
        }

        public string PreferredDataLocation
        {
            get
            {
                return GetProperty("PreferredDataLocation");
            }
        }

        public string Office
        {
            get
            {
                return GetProperty("Office");
            }
        }

        public string Department
        {
            get
            { 
                return GetProperty("Department");
            }
        }

        public string FirstName
        {
            get
            {
                return GetProperty("FirstName");
            }
        }

        public string LastName
        {
            get
            {
                return GetProperty("LastName");
            }
        }

        public string ManagementLevel
        {
            get
            {
                return GetProperty("ManagementLevel");
            }
        }

        public string Function
        {
            get
            {
                return GetProperty("Function");
            }
        }

        public string PasswordLastSet
        {
            get
            {
                return GetProperty("PasswordLastSet");
            }
            set
            {
                SetProperty("PasswordLastSet", value);
            }
        }

        internal string GetProperty(string key)
        {
            if (UserProfileProperties == null) return string.Empty;
            var property = UserProfileProperties.FirstOrDefault(x => x.Key == key);
            if (property == null) return string.Empty;
            return property.Value;
        }
        internal void SetProperty(string key, string value)
        {
            if (UserProfileProperties == null) return;
            var property = UserProfileProperties.FirstOrDefault(x => x.Key == key);
            if (property == null) return;
            property.Value = value;
        }

    }

    #endregion


}
