using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EVERFI.Foundry.Classes
{
    // TODO: only certain roles available for certain types (Ex. Admin = Primary, FacStaffLearner = Supervisor or Nonsupervisor
    internal enum Types
    {
        [Description("he_learner")]
        HELearner = 1,
        [Description("he_admin")]
        HEAdmin,
        [Description("fac_staff_learner")]
        FacStaffLearner,
        [Description("fac_staff_admin")]
        FacStaffAdmin,
        [Description("cc_learner")]
        CCLearner,
        [Description("cc_admin")]
        CCAdmin,
        [Description("next_learner")]
        AdultFinancialLearner,
        [Description("at_work_manager")]
        AdultFinancialManager,
        [Description("event_volunteer")]
        EventVolunteer,
        [Description("event_manager")]
        EventManager = 0
    };
    internal enum Roles
    {
        [Description("undergrad")]
        Undergraduate = 1,
        [Description("graduate")]
        Graduate,
        [Description("non_traditional")]
        NonTraditional,
        [Description("greek")]
        Greek,
        [Description("primary")]
        Primary,
        [Description("secondary")]
        Secondary,
        [Description("supervisor")]
        Supervisor,
        [Description("non_supervisor")]
        NonSupervisor,
        [Description("default")]
        Default
    };
    public enum UserRole // Concatenation of ROLE + TYPE
    {
        [Description("Undergraduate Learner")]
        UndergraduateHE = 11,
        [Description("Graduate Learner")]
        GraduateHE = 21,
        [Description("Non-Traditional Learner")]
        NonTraditionalHE = 31,
        [Description("Greek Learner")]
        GreekHE = 41,
        [Description("Primary Admin")]
        HEAdmin = 52,
        [Description("Faculty/Staff Supervisor")]
        FacStaffSupervisor = 63,
        [Description("Faculty/Staff Nonsupervisor")]
        FacStaffNonSupervisor = 73,
        [Description("Faculty/Staff Admin")]
        FacStaffAdmin = 54,
        [Description("Employee Supervisor")]
        CodeConductSupervisor = 65,
        [Description("Employee Nonsupervisor")]
        CodeConductNonSupervisor = 75,
        [Description("Employee Admin")]
        CodeConductAdmin = 55,
        [Description("Financial Learner")]
        AdultFinancialLearner = 87,
        [Description("Financial Admin")]
        AdultFinancialAdmin = 58,
        [Description("Events Volunteer")]
        EventVolunteer = 89,
        [Description("Events Admin")]
        EventManager = 50
    }

    public class UserType
    {
        internal Types Type { get; set; }
        internal Roles Role { get; set; }
        public UserRole UserRole { get; set; }

        internal UserType(Types MyType, Roles MyRole)
        {
            this.Type = MyType;
            this.Role = MyRole;
            this.UserRole = (UserRole)int.Parse(((int)MyRole).ToString() + ((int)MyType).ToString());
        }

        public UserType(UserRole MyRole)
        {
            this.UserRole = MyRole;
            this.Type = (Types)((int)MyRole % 10);
            this.Role = (Roles)((int)MyRole / 10);
        }

        internal static string GetDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? value.ToString();
        }

        internal static E GetValueFromDescription<E>(string description) where E : Enum
        {
            foreach (var field in typeof(E).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (E)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (E)field.GetValue(null);
                }
            }

            throw new ArgumentException("Description not valid", nameof(description));
           
        }
    
     
        
    }
}
