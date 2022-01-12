using System;
using System.Data;
using System.Runtime.Serialization;
using System.Collections.Generic;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using MedioClinicCustomizations.DataProtection.ConsentCustomizations;

[assembly: RegisterObjectType(typeof(ConsentCookieLevelInfo), ConsentCookieLevelInfo.OBJECT_TYPE)]

namespace MedioClinicCustomizations.DataProtection.ConsentCustomizations
{
    /// <summary>
    /// Data container class for <see cref="ConsentCookieLevelInfo"/>.
    /// </summary>
    [Serializable]
    public class ConsentCookieLevelInfo : AbstractInfo<ConsentCookieLevelInfo, IConsentCookieLevelInfoProvider>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "medioclinic.consentcookielevel";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(ConsentCookieLevelInfoProvider), OBJECT_TYPE, "MedioClinic.ConsentCookieLevel", "ConsentCookieLevelID", "ConsentCookieLevelLastModified", "ConsentCookieLevelGuid", null, null, null, null, null, null)
        {
            ModuleName = "MedioClinic.ConsentCustomizations",
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("ConsentID", "cms.consent", ObjectDependencyEnum.Required),
            },
        };


        /// <summary>
        /// Consent cookie level ID.
        /// </summary>
        [DatabaseField]
        public virtual int ConsentCookieLevelID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("ConsentCookieLevelID"), 0);
            }
            set
            {
                SetValue("ConsentCookieLevelID", value);
            }
        }


        /// <summary>
        /// Consent ID.
        /// </summary>
        [DatabaseField]
        public virtual int ConsentID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("ConsentID"), 0);
            }
            set
            {
                SetValue("ConsentID", value);
            }
        }


        /// <summary>
        /// Cookie level.
        /// </summary>
        [DatabaseField]
        public virtual int CookieLevel
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("CookieLevel"), 0);
            }
            set
            {
                SetValue("CookieLevel", value, 0);
            }
        }


        /// <summary>
        /// Consent cookie level guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid ConsentCookieLevelGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("ConsentCookieLevelGuid"), Guid.Empty);
            }
            set
            {
                SetValue("ConsentCookieLevelGuid", value);
            }
        }


        /// <summary>
        /// Consent cookie level last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime ConsentCookieLevelLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("ConsentCookieLevelLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("ConsentCookieLevelLastModified", value);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            Provider.Delete(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            Provider.Set(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected ConsentCookieLevelInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="ConsentCookieLevelInfo"/> class.
        /// </summary>
        public ConsentCookieLevelInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="ConsentCookieLevelInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public ConsentCookieLevelInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}