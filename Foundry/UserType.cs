using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    public enum Types
    {
        [Description("he_learner")]
        HELearner,
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
        EventManager
    };

    public enum Roles
    {
        [Description("undergrad")]
        Undergraduate,
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

    public class UserType
    {
        public Types Type { get; set; }
        public Roles Role { get; set; }

        public UserType(Types MyType, Roles MyRole)
        {
            this.Type = MyType;
            this.Role = MyRole;
        }

        public static string GetDescription(Enum value)
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
    }
}
