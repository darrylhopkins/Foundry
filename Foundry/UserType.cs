using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
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
