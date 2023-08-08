using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFAuto.DAL;

namespace TFAuto.Domain;

public interface IRegistrationService
{
    public Task<UserRegistrationResponse> RegisrateUser(UserRegistrationRequest userRequest);
}
