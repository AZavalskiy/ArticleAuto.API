using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.Domain;

public interface IAuthenticationService
{
    public ValueTask<LoginResponse> LoginAsync(LoginRequest loginCredentials);
    public ValueTask<LoginResponse> GetNewTokensByRefreshAsync(RefreshRequest refreshToken);
}
