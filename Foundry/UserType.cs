using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
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

        internal static Types StringToType(string value)
        {
            switch (value)
            {
                case "Higher Education Student":
                    return Types.HELearner;
                case "High Education Training Admin":
                    return Types.HEAdmin;
                case "Faculty/Staff Admin":
                    return Types.FacStaffAdmin;
                case "Faculty/Staff Learner":
                    return Types.FacStaffLearner;
                case "Employee Learner":
                    return Types.CCLearner;
                case "Employee Training Admin":
                    return Types.CCAdmin;
                case "Events Admin":
                    return Types.EventManager;
                case "Events Volunteer":
                    return Types.EventVolunteer;
                // TODO: Add Financial Learner and Manager
                default:
                    return Types.HELearner; // TODO: return something for default
            }
        }

        internal static Roles StringToRole(string value)
        {
            switch (value)
            {
                case "Undergrad":
                    return Roles.Undergraduate;
                case "Primary":
                    return Roles.Primary;
                case "Graduate":
                    return Roles.Graduate;
                case "Greek":
                    return Roles.Greek;
                case "Non-supervisor": // Check to make sure the s is not capitalized
                    return Roles.NonSupervisor;
                case "Non-Traditional":
                    return Roles.NonTraditional;
                case "Supervisor":
                    return Roles.Supervisor;
                case "Default":
                    return Roles.Default;
                default:
                    return Roles.Default; // TODO: return something for default
            }
        }
    }
}
